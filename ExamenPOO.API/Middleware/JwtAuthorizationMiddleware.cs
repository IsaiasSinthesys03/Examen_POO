using System.Net;
using System.Text.Json;

namespace ExamenPOO.API.Middleware
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthorizationMiddleware> _logger;

        public JwtAuthorizationMiddleware(RequestDelegate next, ILogger<JwtAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Permitir endpoints públicos
            var publicEndpoints = new[]
            {
                "/swagger",
                "/api/auth/login",
                "/api/auth/register",
                "/",
                "/_framework",
                "/_vs",
                "/favicon.ico"
            };

            var path = context.Request.Path.Value?.ToLower() ?? "";
            var isPublicEndpoint = publicEndpoints.Any(endpoint => path.StartsWith(endpoint.ToLower()));

            if (!isPublicEndpoint)
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                
                // Verificar si el header Authorization está presente
                if (string.IsNullOrEmpty(authHeader))
                {
                    _logger.LogWarning("Intento de acceso sin token JWT desde IP: {IP} a endpoint: {Endpoint}", 
                        context.Connection.RemoteIpAddress, context.Request.Path);
                    
                    await WriteUnauthorizedResponse(context, "Token JWT requerido", "NO_TOKEN");
                    return;
                }

                // Verificar si el token tiene el formato correcto
                if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Formato de token JWT inválido desde IP: {IP} a endpoint: {Endpoint}", 
                        context.Connection.RemoteIpAddress, context.Request.Path);
                    
                    await WriteUnauthorizedResponse(context, "Formato de token inválido. Use 'Bearer <token>'", "INVALID_TOKEN_FORMAT");
                    return;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                // Verificar si el token no está vacío
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token JWT vacío desde IP: {IP} a endpoint: {Endpoint}", 
                        context.Connection.RemoteIpAddress, context.Request.Path);
                    
                    await WriteUnauthorizedResponse(context, "Token JWT vacío", "EMPTY_TOKEN");
                    return;
                }
            }

            await _next(context);
        }

        private async Task WriteUnauthorizedResponse(HttpContext context, string message, string errorCode)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized Access",
                Status = 401,
                Detail = message,
                Instance = context.Request.Path.Value,
                ErrorCode = errorCode,
                Timestamp = DateTime.UtcNow,
                SecurityInfo = new
                {
                    Message = "Esta API requiere autenticación JWT válida para acceder a recursos protegidos.",
                    Instructions = new[]
                    {
                        "1. Obtenga un token JWT válido usando el endpoint /api/auth/login",
                        "2. Incluya el token en el header Authorization: Bearer <su-token>",
                        "3. Asegúrese de que el token no haya expirado",
                        "4. Verifique que el token tenga el formato correcto"
                    },
                    LoginEndpoint = "/api/auth/login",
                    TokenExpiry = "Los tokens JWT tienen una duración limitada y deben renovarse periódicamente"
                },
                RequestInfo = new
                {
                    Path = context.Request.Path.Value,
                    Method = context.Request.Method,
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                    RemoteIP = context.Connection.RemoteIpAddress?.ToString()
                }
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
