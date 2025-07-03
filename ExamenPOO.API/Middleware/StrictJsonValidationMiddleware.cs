using System.Text;
using System.Text.Json;
using ExamenPOO.Application.Services;

namespace ExamenPOO.API.Middleware;

/// <summary>
/// Middleware que intercepta las solicitudes y valida que el JSON recibido
/// contenga tipos exactos sin conversiones implícitas
/// </summary>
public class StrictJsonValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StrictJsonValidationMiddleware> _logger;

    public StrictJsonValidationMiddleware(
        RequestDelegate next, 
        ILogger<StrictJsonValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Solo procesar solicitudes POST, PUT, PATCH con contenido JSON
        if (ShouldValidateRequest(context))
        {
            var validationResult = await ValidateRequestBodyAsync(context);
            if (!validationResult.IsValid)
            {
                await WriteValidationErrorResponse(context, validationResult);
                return;
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Determina si la solicitud debe ser validada
    /// </summary>
    private static bool ShouldValidateRequest(HttpContext context)
    {
        var method = context.Request.Method.ToUpperInvariant();
        var isRelevantMethod = method == "POST" || method == "PUT" || method == "PATCH";
        var hasJsonContent = context.Request.ContentType?.Contains("application/json") == true;
        var hasBody = context.Request.ContentLength > 0;

        return isRelevantMethod && hasJsonContent && hasBody;
    }

    /// <summary>
    /// Valida el cuerpo de la solicitud JSON para tipos estrictos
    /// </summary>
    private async Task<JsonValidationResult> ValidateRequestBodyAsync(HttpContext context)
    {
        try
        {
            // Leer el cuerpo de la solicitud
            context.Request.EnableBuffering();
            var bodyStream = context.Request.Body;
            bodyStream.Position = 0;

            using var reader = new StreamReader(bodyStream, Encoding.UTF8, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            bodyStream.Position = 0;

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return JsonValidationResult.Success();
            }

            // Analizar el JSON para detectar violaciones de tipo
            var validationErrors = AnalyzeJsonForTypeViolations(requestBody);
            
            if (validationErrors.Any())
            {
                return JsonValidationResult.Failure(validationErrors);
            }

            return JsonValidationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during JSON validation");
            return JsonValidationResult.Success(); // Permitir que otros validadores manejen errores de JSON malformado
        }
    }

    /// <summary>
    /// Analiza el JSON raw para detectar violaciones de tipo antes del model binding
    /// </summary>
    private List<JsonTypeViolation> AnalyzeJsonForTypeViolations(string jsonString)
    {
        var violations = new List<JsonTypeViolation>();

        try
        {
            using var document = JsonDocument.Parse(jsonString);
            AnalyzeJsonElement(document.RootElement, "", violations);
        }
        catch (JsonException)
        {
            // JSON malformado será manejado por otros validadores
            return violations;
        }

        return violations;
    }

    /// <summary>
    /// Analiza recursivamente elementos JSON para detectar violaciones de tipo
    /// </summary>
    private void AnalyzeJsonElement(JsonElement element, string path, List<JsonTypeViolation> violations)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var propertyPath = string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}";
                    AnalyzeJsonElement(property.Value, propertyPath, violations);
                }
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var arrayPath = $"{path}[{index}]";
                    AnalyzeJsonElement(item, arrayPath, violations);
                    index++;
                }
                break;

            case JsonValueKind.String:
                ValidateStringValue(element, path, violations);
                break;

            case JsonValueKind.Number:
                ValidateNumberValue(element, path, violations);
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                ValidateBooleanValue(element, path, violations);
                break;
        }
    }

    /// <summary>
    /// Valida valores de string para detectar violaciones de tipo
    /// </summary>
    private void ValidateStringValue(JsonElement element, string path, List<JsonTypeViolation> violations)
    {
        var stringValue = element.GetString();
        if (string.IsNullOrEmpty(stringValue)) return;

        var fieldName = GetFieldName(path);
        var fieldNameLower = fieldName.ToLowerInvariant();

        // Verificar si campos de texto contienen solo números
        if (IsTextFieldName(fieldNameLower) && IsNumericString(stringValue))
        {
            violations.Add(new JsonTypeViolation
            {
                Path = path,
                FieldName = fieldName,
                Value = stringValue,
                ViolationType = "PurelyNumericText",
                Message = $"Field '{fieldName}' cannot contain purely numeric values. Expected alphabetic text.",
                ExpectedFormat = "Alphabetic characters and spaces only",
                ReceivedValue = stringValue
            });
        }

        // Verificar emails con formato incorrecto
        if (IsEmailFieldName(fieldNameLower) && IsNumericString(stringValue))
        {
            violations.Add(new JsonTypeViolation
            {
                Path = path,
                FieldName = fieldName,
                Value = stringValue,
                ViolationType = "NumericEmail",
                Message = $"Field '{fieldName}' cannot be a numeric value. Expected valid email format.",
                ExpectedFormat = "user@domain.com",
                ReceivedValue = stringValue
            });
        }

        // Verificar códigos con formato incorrecto
        if (IsCodeFieldName(fieldNameLower))
        {
            var violation = ValidateCodeFormat(fieldName, stringValue);
            if (violation != null)
            {
                violation.Path = path;
                violations.Add(violation);
            }
        }
    }

    /// <summary>
    /// Valida valores numéricos para detectar contextos incorrectos
    /// </summary>
    private void ValidateNumberValue(JsonElement element, string path, List<JsonTypeViolation> violations)
    {
        var fieldName = GetFieldName(path);
        var fieldNameLower = fieldName.ToLowerInvariant();

        // Verificar si se envían números donde se esperan strings de texto
        if (IsTextFieldName(fieldNameLower))
        {
            violations.Add(new JsonTypeViolation
            {
                Path = path,
                FieldName = fieldName,
                Value = element.ToString(),
                ViolationType = "NumericInTextField",
                Message = $"Field '{fieldName}' expects text, but received a number. Numbers are not allowed in text fields.",
                ExpectedFormat = "Alphabetic text only",
                ReceivedValue = element.ToString()
            });
        }
    }

    /// <summary>
    /// Valida valores booleanos para detectar contextos incorrectos
    /// </summary>
    private void ValidateBooleanValue(JsonElement element, string path, List<JsonTypeViolation> violations)
    {
        var fieldName = GetFieldName(path);
        var fieldNameLower = fieldName.ToLowerInvariant();

        // Verificar si se envían booleanos donde se esperan otros tipos
        if (IsTextFieldName(fieldNameLower) || IsEmailFieldName(fieldNameLower) || IsCodeFieldName(fieldNameLower))
        {
            violations.Add(new JsonTypeViolation
            {
                Path = path,
                FieldName = fieldName,
                Value = element.ToString(),
                ViolationType = "BooleanInWrongContext",
                Message = $"Field '{fieldName}' expects a different type, but received a boolean value.",
                ExpectedFormat = GetExpectedFormatForField(fieldNameLower),
                ReceivedValue = element.ToString()
            });
        }
    }

    /// <summary>
    /// Valida formato específico de códigos
    /// </summary>
    private JsonTypeViolation? ValidateCodeFormat(string fieldName, string value)
    {
        var fieldNameLower = fieldName.ToLowerInvariant();

        if (fieldNameLower.Contains("course") && fieldNameLower.Contains("code"))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{2,4}\d{3,4}$"))
            {
                return new JsonTypeViolation
                {
                    FieldName = fieldName,
                    Value = value,
                    ViolationType = "InvalidCourseCode",
                    Message = $"Field '{fieldName}' must follow course code format.",
                    ExpectedFormat = "2-4 uppercase letters followed by 3-4 digits (e.g., COMP1234)",
                    ReceivedValue = value
                };
            }
        }

        if (fieldNameLower.Contains("student") && fieldNameLower.Contains("number"))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z0-9]{6,20}$"))
            {
                return new JsonTypeViolation
                {
                    FieldName = fieldName,
                    Value = value,
                    ViolationType = "InvalidStudentNumber",
                    Message = $"Field '{fieldName}' must follow student number format.",
                    ExpectedFormat = "6-20 uppercase letters and numbers (e.g., STUD123456)",
                    ReceivedValue = value
                };
            }
        }

        return null;
    }

    /// <summary>
    /// Escribe respuesta de error de validación JSON
    /// </summary>
    private async Task WriteValidationErrorResponse(HttpContext context, JsonValidationResult validationResult)
    {
        var response = new
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Strict JSON Type Validation Failed",
            Status = 400,
            Detail = "The JSON payload contains type violations that prevent strict validation compliance.",
            Instance = context.Request.Path,
            Violations = validationResult.Violations.Select(v => new
            {
                path = v.Path,
                field = v.FieldName,
                violationType = v.ViolationType,
                message = v.Message,
                expectedFormat = v.ExpectedFormat,
                receivedValue = v.ReceivedValue
            }).ToArray(),
            StrictValidationRules = new
            {
                TextFields = "Must contain only alphabetic characters and spaces, no numeric values",
                EmailFields = "Must be valid email format, not numeric values",
                CodeFields = "Must follow specific patterns (e.g., course codes: COMP1234)",
                NumericFields = "Must be genuine numbers in appropriate contexts",
                TypeCoercion = "Automatic type conversions are not allowed"
            }
        };

        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    #region Métodos de utilidad

    private static string GetFieldName(string path)
    {
        return path.Split('.').LastOrDefault()?.Split('[').FirstOrDefault() ?? path;
    }

    private static bool IsTextFieldName(string fieldName)
    {
        var textFields = new[] { "name", "firstname", "lastname", "career", "department", "title" };
        return textFields.Any(field => fieldName.Contains(field));
    }

    private static bool IsEmailFieldName(string fieldName)
    {
        return fieldName.Contains("email");
    }

    private static bool IsCodeFieldName(string fieldName)
    {
        return fieldName.Contains("code") || fieldName.Contains("number");
    }

    private static bool IsNumericString(string value)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(value.Trim(), @"^\d+(\.\d+)?$") ||
               (decimal.TryParse(value, out _) && !value.Any(char.IsLetter));
    }

    private static string GetExpectedFormatForField(string fieldName)
    {
        if (IsTextFieldName(fieldName)) return "Alphabetic text only";
        if (IsEmailFieldName(fieldName)) return "Valid email format (user@domain.com)";
        if (IsCodeFieldName(fieldName)) return "Alphanumeric code format";
        return "Appropriate data type for field context";
    }

    #endregion
}

/// <summary>
/// Resultado de validación JSON
/// </summary>
public class JsonValidationResult
{
    public bool IsValid { get; private set; }
    public List<JsonTypeViolation> Violations { get; private set; } = new();

    private JsonValidationResult(bool isValid, List<JsonTypeViolation>? violations = null)
    {
        IsValid = isValid;
        Violations = violations ?? new List<JsonTypeViolation>();
    }

    public static JsonValidationResult Success()
    {
        return new JsonValidationResult(true);
    }

    public static JsonValidationResult Failure(List<JsonTypeViolation> violations)
    {
        return new JsonValidationResult(false, violations);
    }
}

/// <summary>
/// Violación de tipo en JSON
/// </summary>
public class JsonTypeViolation
{
    public string Path { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ExpectedFormat { get; set; } = string.Empty;
    public string ReceivedValue { get; set; } = string.Empty;
}
