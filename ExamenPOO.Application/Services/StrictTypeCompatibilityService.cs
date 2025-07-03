using System.Reflection;
using System.Text.RegularExpressions;

namespace ExamenPOO.Application.Services;

/// <summary>
/// Servicio que determina si un valor es estrictamente compatible con un tipo esperado
/// aplicando reglas de negocio específicas más allá de la verificación básica de tipos
/// </summary>
public class StrictTypeCompatibilityService
{
    private readonly Dictionary<string, Func<object, bool>> _businessRules;

    public StrictTypeCompatibilityService()
    {
        _businessRules = InitializeBusinessRules();
    }

    /// <summary>
    /// Verifica si un valor es estrictamente compatible con un tipo esperado
    /// considerando reglas de negocio específicas
    /// </summary>
    /// <param name="value">Valor a verificar</param>
    /// <param name="expectedType">Tipo esperado</param>
    /// <param name="fieldName">Nombre del campo para aplicar reglas de contexto</param>
    /// <returns>Resultado de la verificación de compatibilidad</returns>
    public TypeCompatibilityResult IsStrictlyCompatible(object value, Type expectedType, string fieldName)
    {
        if (value == null)
        {
            return TypeCompatibilityResult.Success();
        }

        var actualType = value.GetType();
        var fieldNameLower = fieldName.ToLowerInvariant();

        // 1. Verificar compatibilidad básica de tipos
        var basicCompatibility = CheckBasicTypeCompatibility(value, expectedType, actualType);
        if (!basicCompatibility.IsCompatible)
        {
            return basicCompatibility;
        }

        // 2. Aplicar reglas de negocio específicas por contexto de campo
        var businessRuleResult = ApplyBusinessRulesForField(value, fieldNameLower);
        if (!businessRuleResult.IsCompatible)
        {
            return businessRuleResult;
        }

        // 3. Verificar patrones específicos según el tipo de campo
        var patternResult = ValidateFieldPatterns(value, fieldNameLower);
        if (!patternResult.IsCompatible)
        {
            return patternResult;
        }

        return TypeCompatibilityResult.Success();
    }

    /// <summary>
    /// Verifica la compatibilidad básica de tipos sin conversiones implícitas
    /// </summary>
    private TypeCompatibilityResult CheckBasicTypeCompatibility(object value, Type expectedType, Type actualType)
    {
        var underlyingExpectedType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
        var underlyingActualType = Nullable.GetUnderlyingType(actualType) ?? actualType;

        // Verificación estricta: tipos deben coincidir exactamente
        if (underlyingActualType != underlyingExpectedType)
        {
            // Casos específicos donde las conversiones no están permitidas
            if (IsNumericType(underlyingExpectedType) && actualType == typeof(string))
            {
                return TypeCompatibilityResult.Failure(
                    $"Expected {underlyingExpectedType.Name} but received string. " +
                    $"String-to-number conversions are not allowed in strict mode.");
            }

            if (underlyingExpectedType == typeof(string) && IsNumericType(underlyingActualType))
            {
                return TypeCompatibilityResult.Failure(
                    $"Expected string but received {underlyingActualType.Name}. " +
                    $"Number-to-string conversions are not allowed in strict mode.");
            }

            if (underlyingExpectedType == typeof(DateTime) && actualType == typeof(string))
            {
                return TypeCompatibilityResult.Failure(
                    $"Expected DateTime object but received string. " +
                    $"String-to-DateTime conversions are not allowed in strict mode.");
            }

            if (underlyingExpectedType == typeof(bool) && actualType == typeof(string))
            {
                return TypeCompatibilityResult.Failure(
                    $"Expected boolean but received string. " +
                    $"String-to-boolean conversions are not allowed in strict mode.");
            }

            return TypeCompatibilityResult.Failure(
                $"Type mismatch: expected {underlyingExpectedType.Name} but received {actualType.Name}.");
        }

        return TypeCompatibilityResult.Success();
    }

    /// <summary>
    /// Aplica reglas de negocio específicas según el contexto del campo
    /// </summary>
    private TypeCompatibilityResult ApplyBusinessRulesForField(object value, string fieldName)
    {
        if (value == null) return TypeCompatibilityResult.Success();

        // Verificar si hay reglas específicas para este campo
        var applicableRules = _businessRules.Where(rule => fieldName.Contains(rule.Key)).ToList();

        foreach (var rule in applicableRules)
        {
            if (!rule.Value(value))
            {
                return TypeCompatibilityResult.Failure(
                    $"Field '{fieldName}' violates business rule: {GetBusinessRuleDescription(rule.Key)}");
            }
        }

        return TypeCompatibilityResult.Success();
    }

    /// <summary>
    /// Valida patrones específicos según el tipo de campo
    /// </summary>
    private TypeCompatibilityResult ValidateFieldPatterns(object value, string fieldName)
    {
        if (value == null) return TypeCompatibilityResult.Success();

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
        {
            return TypeCompatibilityResult.Success();
        }

        // Validaciones específicas por tipo de campo
        if (IsPersonNameField(fieldName))
        {
            return ValidatePersonNamePattern(stringValue, fieldName);
        }

        if (IsEmailField(fieldName))
        {
            return ValidateEmailPattern(stringValue, fieldName);
        }

        if (IsCodeField(fieldName))
        {
            return ValidateCodePattern(stringValue, fieldName);
        }

        if (IsPhoneField(fieldName))
        {
            return ValidatePhonePattern(stringValue, fieldName);
        }

        if (IsDescriptionField(fieldName))
        {
            return ValidateDescriptionPattern(stringValue, fieldName);
        }

        return TypeCompatibilityResult.Success();
    }

    /// <summary>
    /// Inicializa las reglas de negocio específicas por tipo de campo
    /// </summary>
    private Dictionary<string, Func<object, bool>> InitializeBusinessRules()
    {
        return new Dictionary<string, Func<object, bool>>
        {
            ["name"] = value => !IsNumericString(value.ToString()!),
            ["firstname"] = value => !IsNumericString(value.ToString()!),
            ["lastname"] = value => !IsNumericString(value.ToString()!),
            ["career"] = value => !IsNumericString(value.ToString()!),
            ["department"] = value => !IsNumericString(value.ToString()!),
            ["email"] = value => !IsNumericString(value.ToString()!) && IsValidEmailFormat(value.ToString()!),
            ["phone"] = value => IsValidPhoneFormat(value.ToString()!),
            ["number"] = value => IsValidCodeFormat(value.ToString()!),
            ["code"] = value => IsValidCodeFormat(value.ToString()!),
            ["hours"] = value => value is int intValue && intValue >= 1 && intValue <= 6,
            ["credits"] = value => value is int intValue && intValue >= 1 && intValue <= 6
        };
    }

    #region Validaciones de patrones específicos

    private TypeCompatibilityResult ValidatePersonNamePattern(string value, string fieldName)
    {
        if (IsNumericString(value))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' cannot contain purely numeric values. Person names must contain alphabetic characters.");
        }

        if (!Regex.IsMatch(value, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' must contain only alphabetic characters and spaces. No numbers or special characters allowed.");
        }

        return TypeCompatibilityResult.Success();
    }

    private TypeCompatibilityResult ValidateEmailPattern(string value, string fieldName)
    {
        if (IsNumericString(value))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' cannot be a numeric value. A valid email address is required.");
        }

        if (!IsValidEmailFormat(value))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' must be a valid email address format (e.g., user@domain.com).");
        }

        return TypeCompatibilityResult.Success();
    }

    private TypeCompatibilityResult ValidateCodePattern(string value, string fieldName)
    {
        if (fieldName.Contains("course") && !Regex.IsMatch(value, @"^[A-Z]{2,4}\d{3,4}$"))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' must follow course code format: 2-4 uppercase letters followed by 3-4 digits (e.g., COMP1234).");
        }

        if (fieldName.Contains("student") && !Regex.IsMatch(value, @"^[A-Z0-9]{6,20}$"))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' must follow student number format: 6-20 uppercase letters and numbers (e.g., STUD123456).");
        }

        return TypeCompatibilityResult.Success();
    }

    private TypeCompatibilityResult ValidatePhonePattern(string value, string fieldName)
    {
        if (!Regex.IsMatch(value, @"^[\d\s\-\+\(\)]+$"))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' must contain only numbers, spaces, hyphens, plus signs, and parentheses.");
        }

        return TypeCompatibilityResult.Success();
    }

    private TypeCompatibilityResult ValidateDescriptionPattern(string value, string fieldName)
    {
        if (IsNumericString(value))
        {
            return TypeCompatibilityResult.Failure(
                $"Field '{fieldName}' cannot be purely numeric. Descriptions must contain descriptive text.");
        }

        return TypeCompatibilityResult.Success();
    }

    #endregion

    #region Métodos de utilidad para identificar tipos de campos

    private static bool IsPersonNameField(string fieldName)
    {
        var nameFields = new[] { "name", "firstname", "lastname", "career", "department" };
        return nameFields.Any(field => fieldName.Contains(field));
    }

    private static bool IsEmailField(string fieldName)
    {
        return fieldName.Contains("email");
    }

    private static bool IsCodeField(string fieldName)
    {
        return fieldName.Contains("code") || fieldName.Contains("number");
    }

    private static bool IsPhoneField(string fieldName)
    {
        return fieldName.Contains("phone");
    }

    private static bool IsDescriptionField(string fieldName)
    {
        var descriptionFields = new[] { "description", "address", "prerequisites", "notes" };
        return descriptionFields.Any(field => fieldName.Contains(field));
    }

    #endregion

    #region Métodos de validación de formato

    private static bool IsNumericString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Regex.IsMatch(value.Trim(), @"^\d+(\.\d+)?$") ||
               (decimal.TryParse(value, out _) && !value.Any(char.IsLetter));
    }

    private static bool IsValidEmailFormat(string value)
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(value, emailPattern);
    }

    private static bool IsValidPhoneFormat(string value)
    {
        return Regex.IsMatch(value, @"^[\d\s\-\+\(\)]+$");
    }

    private static bool IsValidCodeFormat(string value)
    {
        return Regex.IsMatch(value, @"^[A-Z0-9]+$");
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
               type == typeof(byte) || type == typeof(sbyte) || type == typeof(uint) ||
               type == typeof(ulong) || type == typeof(ushort) || type == typeof(float) ||
               type == typeof(double) || type == typeof(decimal);
    }

    #endregion

    #region Métodos de descripción de reglas

    private static string GetBusinessRuleDescription(string ruleKey)
    {
        return ruleKey switch
        {
            "name" => "Names cannot contain purely numeric values",
            "firstname" => "First names must contain alphabetic characters only",
            "lastname" => "Last names must contain alphabetic characters only",
            "career" => "Career names must contain alphabetic characters only",
            "department" => "Department names must contain alphabetic characters only",
            "email" => "Email must be a valid email format and not purely numeric",
            "phone" => "Phone numbers must contain only valid phone characters",
            "number" => "Numbers and codes must follow specific formatting rules",
            "code" => "Codes must follow specific formatting rules",
            "hours" => "Hours must be between 1 and 6",
            "credits" => "Credits must be between 1 and 6",
            _ => "Value must comply with business rules"
        };
    }

    #endregion
}

/// <summary>
/// Resultado de la verificación de compatibilidad de tipos
/// </summary>
public class TypeCompatibilityResult
{
    public bool IsCompatible { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private TypeCompatibilityResult(bool isCompatible, string errorMessage = "")
    {
        IsCompatible = isCompatible;
        ErrorMessage = errorMessage;
    }

    public static TypeCompatibilityResult Success()
    {
        return new TypeCompatibilityResult(true);
    }

    public static TypeCompatibilityResult Failure(string errorMessage)
    {
        return new TypeCompatibilityResult(false, errorMessage);
    }
}
