using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ExamenPOO.Application.Validation;

namespace ExamenPOO.API.Filters;

/// <summary>
/// Filtro de acción que realiza validación estricta de tipos antes del procesamiento
/// </summary>
public class StrictTypeValidationFilter : IActionFilter
{
    private readonly DynamicStrictValidator _dynamicValidator;

    public StrictTypeValidationFilter()
    {
        _dynamicValidator = new DynamicStrictValidator();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var validationErrors = new List<ValidationError>();

        // Validar todos los argumentos de acción
        foreach (var argument in context.ActionArguments)
        {
            var parameterName = argument.Key;
            var parameterValue = argument.Value;

            if (parameterValue == null) continue;

            // Obtener información del parámetro esperado
            var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            var methodInfo = actionDescriptor?.MethodInfo;
            var expectedParameter = methodInfo?.GetParameters()
                .FirstOrDefault(p => p.Name == parameterName);

            if (expectedParameter != null)
            {
                // Validar compatibilidad estricta de tipos
                var typeCompatibilityError = ValidateStrictTypeCompatibility(
                    parameterValue, expectedParameter.ParameterType, parameterName);
                
                if (typeCompatibilityError != null)
                {
                    validationErrors.Add(typeCompatibilityError);
                    continue;
                }
            }

            // Aplicar validación dinámica para DTOs y objetos complejos
            if (IsComplexType(parameterValue.GetType()))
            {
                var dynamicValidationResults = _dynamicValidator.ValidateObject(parameterValue);
                foreach (var result in dynamicValidationResults)
                {
                    validationErrors.Add(new ValidationError
                    {
                        PropertyName = result.MemberNames.FirstOrDefault() ?? parameterName,
                        ErrorMessage = result.ErrorMessage ?? "Validation failed",
                        AttemptedValue = GetPropertyValue(parameterValue, result.MemberNames.FirstOrDefault() ?? "")?.ToString()
                    });
                }
            }

            // Validar atributos de validación estándar y personalizados
            var attributeValidationErrors = ValidateDataAnnotations(parameterValue, parameterName);
            validationErrors.AddRange(attributeValidationErrors);
        }

        // Si hay errores, devolver respuesta de error inmediatamente
        if (validationErrors.Any())
        {
            var errorResponse = CreateStrictValidationErrorResponse(validationErrors);
            context.Result = new BadRequestObjectResult(errorResponse);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No se requiere procesamiento post-acción para este filtro
    }

    /// <summary>
    /// Valida la compatibilidad estricta de tipos sin conversiones implícitas
    /// </summary>
    private ValidationError? ValidateStrictTypeCompatibility(object value, Type expectedType, string parameterName)
    {
        var actualType = value.GetType();
        
        // Manejar tipos nullable
        var underlyingExpectedType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
        var underlyingActualType = Nullable.GetUnderlyingType(actualType) ?? actualType;

        // Verificación estricta: el tipo debe coincidir exactamente
        if (underlyingActualType != underlyingExpectedType)
        {
            // Casos especiales donde las conversiones no están permitidas
            if (IsNumericType(underlyingExpectedType) && actualType == typeof(string))
            {
                return new ValidationError
                {
                    PropertyName = parameterName,
                    ErrorMessage = $"The parameter '{parameterName}' expects a {underlyingExpectedType.Name} value, but received a string. String-to-number conversions are not allowed in strict mode.",
                    AttemptedValue = value.ToString(),
                    ExpectedType = underlyingExpectedType.Name,
                    ActualType = actualType.Name
                };
            }

            if (underlyingExpectedType == typeof(string) && IsNumericType(underlyingActualType))
            {
                return new ValidationError
                {
                    PropertyName = parameterName,
                    ErrorMessage = $"The parameter '{parameterName}' expects a string value, but received a {underlyingActualType.Name}. Number-to-string conversions are not allowed in strict mode.",
                    AttemptedValue = value.ToString(),
                    ExpectedType = underlyingExpectedType.Name,
                    ActualType = actualType.Name
                };
            }

            if (underlyingExpectedType == typeof(DateTime) && actualType == typeof(string))
            {
                return new ValidationError
                {
                    PropertyName = parameterName,
                    ErrorMessage = $"The parameter '{parameterName}' expects a DateTime object, but received a string. String-to-DateTime conversions are not allowed in strict mode.",
                    AttemptedValue = value.ToString(),
                    ExpectedType = underlyingExpectedType.Name,
                    ActualType = actualType.Name
                };
            }

            if (underlyingExpectedType == typeof(bool) && actualType == typeof(string))
            {
                return new ValidationError
                {
                    PropertyName = parameterName,
                    ErrorMessage = $"The parameter '{parameterName}' expects a boolean value, but received a string. String-to-boolean conversions are not allowed in strict mode.",
                    AttemptedValue = value.ToString(),
                    ExpectedType = underlyingExpectedType.Name,
                    ActualType = actualType.Name
                };
            }
        }

        return null;
    }

    /// <summary>
    /// Valida atributos de DataAnnotations en el objeto
    /// </summary>
    private List<ValidationError> ValidateDataAnnotations(object obj, string objectName)
    {
        var errors = new List<ValidationError>();
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(obj);

        if (!Validator.TryValidateObject(obj, validationContext, validationResults, true))
        {
            foreach (var result in validationResults)
            {
                var propertyName = result.MemberNames.FirstOrDefault() ?? objectName;
                errors.Add(new ValidationError
                {
                    PropertyName = propertyName,
                    ErrorMessage = result.ErrorMessage ?? "Validation failed",
                    AttemptedValue = GetPropertyValue(obj, propertyName)?.ToString()
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// Crea una respuesta de error estructurada para fallos de validación estricta
    /// </summary>
    private object CreateStrictValidationErrorResponse(List<ValidationError> errors)
    {
        return new
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Strict Type Validation Failed",
            Status = 400,
            Detail = "One or more validation errors occurred. The API requires exact type matching without implicit conversions.",
            Instance = "",
            Errors = errors.GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => new {
                        message = e.ErrorMessage,
                        attemptedValue = e.AttemptedValue,
                        expectedType = e.ExpectedType,
                        actualType = e.ActualType
                    }).ToArray()
                ),
            ValidationRules = new
            {
                StrictTypeMatching = "All parameters must match their expected types exactly. No implicit conversions are allowed.",
                TextFields = "Text fields must contain only alphabetic characters and spaces (no purely numeric values).",
                NumericFields = "Numeric fields must be genuine numeric types, not string representations of numbers.",
                DateFields = "Date fields must be DateTime objects, not string representations.",
                EmailFields = "Email fields must follow valid email format and cannot be numeric values.",
                Examples = new
                {
                    ValidTextInput = "John Doe",
                    InvalidTextInput = "123",
                    ValidNumericInput = 123,
                    InvalidNumericInput = "123",
                    ValidEmailInput = "user@example.com",
                    InvalidEmailInput = "123@456.789"
                }
            }
        };
    }

    /// <summary>
    /// Obtiene el valor de una propiedad de un objeto
    /// </summary>
    private object? GetPropertyValue(object obj, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return obj.ToString();

        try
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(obj);
        }
        catch
        {
            return obj.ToString();
        }
    }

    /// <summary>
    /// Determina si un tipo es un tipo complejo que requiere validación profunda
    /// </summary>
    private static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && 
               type != typeof(string) && 
               type != typeof(DateTime) && 
               type != typeof(decimal) && 
               type != typeof(Guid) &&
               !type.IsEnum;
    }

    /// <summary>
    /// Determina si un tipo es numérico
    /// </summary>
    private static bool IsNumericType(Type type)
    {
        return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
               type == typeof(byte) || type == typeof(sbyte) || type == typeof(uint) ||
               type == typeof(ulong) || type == typeof(ushort) || type == typeof(float) ||
               type == typeof(double) || type == typeof(decimal);
    }
}

/// <summary>
/// Clase para representar errores de validación detallados
/// </summary>
public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? AttemptedValue { get; set; }
    public string? ExpectedType { get; set; }
    public string? ActualType { get; set; }
}
