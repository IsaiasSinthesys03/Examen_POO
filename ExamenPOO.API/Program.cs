using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ExamenPOO.Application.Interfaces;
using ExamenPOO.Application.Services;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Infrastructure.Data;
using ExamenPOO.Infrastructure.Repositories;
using ExamenPOO.Infrastructure.Services;
using ExamenPOO.Infrastructure.UnitOfWork;
using ExamenPOO.API.Filters;
using ExamenPOO.Application.Validation;
using ExamenPOO.API.Middleware;
using ExamenPOO.API.JsonConverters;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Add services to the container.
    builder.Services.AddControllers(options =>
{
    // Agregar filtro de validación estricta de tipos
    options.Filters.Add<StrictTypeValidationFilter>();
})
.AddJsonOptions(options =>
{
    // Configurar opciones JSON para validación estricta
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.AllowTrailingCommas = false;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    
    // Agregar conversores JSON estrictos para prevenir conversiones implícitas
    options.JsonSerializerOptions.Converters.Add(new StrictStringJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictInt32JsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictNullableInt32JsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictBooleanJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictNullableBooleanJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictDateTimeJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictNullableDateTimeJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictDecimalJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new StrictNullableDecimalJsonConverter());
    
    // Configuraciones estrictas adicionales
    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.Strict;
})
.ConfigureApiBehaviorOptions(options =>
{
    // Personalizar respuestas de error de validación
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e => new
            {
                Field = x.Key,
                Message = e.ErrorMessage
            }))
            .ToList();

        var response = new
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Model Validation Failed",
            Status = 400,
            Detail = "One or more validation errors occurred.",
            Instance = context.HttpContext.Request.Path,
            Errors = errors.GroupBy(e => e.Field)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray()),
            StrictValidationInfo = new
            {
                Message = "This API enforces strict type validation. Please ensure all field types match exactly as specified in the API documentation.",
                CommonIssues = new[]
                {
                    "Sending numbers as strings (e.g., \"123\" instead of 123)",
                    "Sending strings for numeric fields",
                    "Using purely numeric values in text fields meant for names",
                    "Incorrect date formats or sending dates as strings"
                }
            }
        };

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
});

// Servicios de validación estricta
builder.Services.AddScoped<DynamicStrictValidator>();
builder.Services.AddScoped<StrictTypeCompatibilityService>();

// Servicio de logging detallado
builder.Services.AddScoped<ExamenPOO.API.Services.IDetailedLoggerService, ExamenPOO.API.Services.DetailedLoggerService>();

// Entity Framework
try
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando Entity Framework: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración de base de datos", ex);
}

// Repository pattern
try
{
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IStudentRepository, StudentRepository>();
    builder.Services.AddScoped<ICourseRepository, CourseRepository>();
    builder.Services.AddScoped<ISemesterEnrollmentRepository, SemesterEnrollmentRepository>();
    builder.Services.AddScoped<IEnrolledCourseRepository, EnrolledCourseRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando repositorios: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración de repositorios", ex);
}

// Application services
try
{
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Services.AddScoped<ICourseService, CourseService>();
    builder.Services.AddScoped<ISemesterEnrollmentService, SemesterEnrollmentService>();
    builder.Services.AddScoped<IJwtService, JwtService>();
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando servicios de aplicación: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración de servicios de aplicación", ex);
}

// Domain services
try
{
    builder.Services.AddScoped<ExamenPOO.Core.Services.IPasswordHashingService, ExamenPOO.Infrastructure.Services.PasswordHashingService>();
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando servicios de dominio: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración de servicios de dominio", ex);
}

// Infrastructure services
try
{
    builder.Services.AddScoped<ExamenPOO.Infrastructure.Services.DatabaseSeederService>();
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando servicios de infraestructura: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración de servicios de infraestructura", ex);
}

// JWT Authentication
try
{
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
    var key = Encoding.ASCII.GetBytes(jwtSecret);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ExamenPOO.API",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ExamenPOO.Client",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Configurar eventos para respuestas personalizadas de autenticación
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Interceptar el challenge por defecto
                context.HandleResponse();

                // Crear respuesta personalizada para token inválido o expirado
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var errorType = "INVALID_TOKEN";
                var errorMessage = "Token JWT inválido o expirado";

                if (context.AuthenticateFailure != null)
                {
                    if (context.AuthenticateFailure.Message.Contains("expired"))
                    {
                        errorType = "EXPIRED_TOKEN";
                        errorMessage = "El token JWT ha expirado";
                    }
                    else if (context.AuthenticateFailure.Message.Contains("signature"))
                    {
                        errorType = "INVALID_SIGNATURE";
                        errorMessage = "Firma del token JWT inválida";
                    }
                }

                var response = new
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Authentication Failed",
                    Status = 401,
                    Detail = errorMessage,
                    Instance = context.Request.Path.Value,
                    ErrorCode = errorType,
                    Timestamp = DateTime.UtcNow,
                    AuthenticationInfo = new
                    {
                        Message = "La autenticación JWT falló. Verifique su token y vuelva a intentarlo.",
                        PossibleCauses = new[]
                        {
                            "Token expirado - obtenga un nuevo token",
                            "Token malformado o corrupto",
                            "Firma del token inválida",
                            "Token no proporcionado en el header Authorization"
                        },
                        Solution = new
                        {
                            LoginEndpoint = "/api/auth/login",
                            HeaderFormat = "Authorization: Bearer <your-jwt-token>",
                            TokenLifetime = "Verifique la fecha de expiración del token"
                        }
                    }
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                return context.Response.WriteAsync(jsonResponse);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Access Forbidden",
                    Status = 403,
                    Detail = "No tiene permisos suficientes para acceder a este recurso",
                    Instance = context.Request.Path.Value,
                    ErrorCode = "INSUFFICIENT_PERMISSIONS",
                    Timestamp = DateTime.UtcNow,
                    AuthorizationInfo = new
                    {
                        Message = "Su token JWT es válido pero no tiene los permisos necesarios para este recurso.",
                        RequiredAction = "Contacte al administrador para obtener los permisos necesarios",
                        UserInfo = new
                        {
                            IsAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false,
                            UserName = context.HttpContext.User.Identity?.Name ?? "Unknown"
                        }
                    }
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                return context.Response.WriteAsync(jsonResponse);
            }
        };
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando JWT Authentication: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración de autenticación JWT", ex);
}

// CORS - Restricción de IP específica
try
{
    var allowedIPs = builder.Configuration.GetSection("AllowedIPs:IPs").Get<string[]>() ?? new[] { "187.155.101.200" };
    var corsEnabled = builder.Configuration.GetValue<bool>("AllowedIPs:Enabled", true);

    builder.Services.AddCors(options =>
    {
        if (corsEnabled)
        {
            options.AddPolicy("AllowSpecificIP", policy =>
            {
                // Construir orígenes permitidos con HTTP y HTTPS
                var allowedOrigins = allowedIPs
                    .SelectMany(ip => new[] { $"http://{ip}", $"https://{ip}" })
                    .Concat(new[] { 
                        "http://www.academicuniversity.somee.com", 
                        "https://www.academicuniversity.somee.com",
                        "https://academicuniversity.somee.com",
                        "http://academicuniversity.somee.com"
                    })
                    .ToArray();
                
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials(); // Permitir credenciales para JWT
            });
        }
        else
        {
            // Política de desarrollo - permitir todo
            options.AddPolicy("AllowSpecificIP", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        }
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando CORS: {ex.Message}");
    // Configuración CORS por defecto en caso de error
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificIP", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

// Swagger/OpenAPI
try
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ExamenPOO API",
            Version = "v1",
            Description = "WebAPI con Clean Architecture para gestión de usuarios y productos"
        });

        // JWT Authorization in Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando el esquema Bearer. Introduce SOLO el token (sin 'Bearer '). Ejemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando Swagger: {ex.Message}");
    // Swagger no es crítico, la app puede funcionar sin él
}

var app = builder.Build();

try
{
    // Configure the HTTP request pipeline.
    
    // Middleware global de manejo de errores (debe ir primero)
    app.UseMiddleware<GlobalExceptionMiddleware>();
    
    // Solo mostrar Swagger en desarrollo o cuando esté explícitamente habilitado
    if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger", false))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExamenPOO API v1");
            c.RoutePrefix = "swagger"; // Mantiene la URL /swagger
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            c.DefaultModelsExpandDepth(-1);
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.EnableValidator();
            c.SupportedSubmitMethods(Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get, 
                                   Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post, 
                                   Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put, 
                                   Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete);
            // Configuración adicional para mejor experiencia
            c.InjectStylesheet("/swagger-ui/custom.css");
            c.DocumentTitle = "ExamenPOO API Documentation";
        });
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles(); // Habilitar archivos estáticos para CSS personalizado

    app.UseCors("AllowSpecificIP");

    // Middleware de validación JSON estricta
    app.UseMiddleware<StrictJsonValidationMiddleware>();

    // Middleware personalizado para advertencias JWT (antes de autenticación)
    app.UseMiddleware<JwtAuthorizationMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    // Redirección automática a Swagger desde la raíz
    app.MapGet("/", () => Results.Redirect("/swagger"));

    app.MapControllers();
}
catch (Exception ex)
{
    Console.WriteLine($"Error configurando pipeline HTTP: {ex.Message}");
    throw new InvalidOperationException("Error crítico en configuración del pipeline HTTP", ex);
}

// Auto migrate database and seed data
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        var seeder = scope.ServiceProvider.GetRequiredService<ExamenPOO.Infrastructure.Services.DatabaseSeederService>();
        await seeder.SeedAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error en migración/seeding de base de datos: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    // No lanzar excepción aquí para permitir que la app continue sin seeding
}

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error crítico iniciando la aplicación: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    throw;
}

}
catch (Exception ex)
{
    Console.WriteLine($"Error crítico durante la configuración inicial: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    throw;
}
