using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExamenPOO.Application.Validation;

/// <summary>
/// Validador genérico que examina dinámicamente las propiedades de cualquier objeto
/// y aplica reglas de validación estrictas según el contexto de cada propiedad
/// </summary>
public class DynamicStrictValidator
{
    private readonly Dictionary<string, Func<object?, string, ValidationResult?>> _propertyValidators;
    private readonly Dictionary<Type, Func<object?, string, ValidationResult?>> _typeValidators;

    public DynamicStrictValidator()
    {
        _propertyValidators = InitializePropertyValidators();
        _typeValidators = InitializeTypeValidators();
    }

    /// <summary>
    /// Valida todas las propiedades de un objeto aplicando reglas estrictas de tipo
    /// </summary>
    /// <param name="obj">Objeto a validar</param>
    /// <returns>Lista de resultados de validación</returns>
    public List<ValidationResult> ValidateObject(object obj)
    {
        var results = new List<ValidationResult>();
        
        if (obj == null)
            return results;

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            var propertyName = property.Name;
            var propertyType = property.PropertyType;

            // Aplicar validaciones específicas por nombre de propiedad
            if (_propertyValidators.ContainsKey(propertyName.ToLowerInvariant()))
            {
                var result = _propertyValidators[propertyName.ToLowerInvariant()](value, propertyName);
                if (result != null)
                    results.Add(result);
            }
            // Aplicar validaciones por tipo
            else if (_typeValidators.ContainsKey(propertyType))
            {
                var result = _typeValidators[propertyType](value, propertyName);
                if (result != null)
                    results.Add(result);
            }
            // Aplicar validaciones genéricas según el contexto del nombre
            else
            {
                var result = ValidateByPropertyContext(value, propertyName, propertyType);
                if (result != null)
                    results.Add(result);
            }
        }

        return results;
    }

    /// <summary>
    /// Inicializa validadores específicos por nombre de propiedad
    /// </summary>
    private Dictionary<string, Func<object?, string, ValidationResult?>> InitializePropertyValidators()
    {
        return new Dictionary<string, Func<object?, string, ValidationResult?>>
        {
            ["firstname"] = ValidatePersonName,
            ["lastname"] = ValidatePersonName,
            ["name"] = ValidatePersonName,
            ["coursename"] = ValidateCourseName,
            ["career"] = ValidateCareerName,
            ["department"] = ValidateDepartmentName,
            ["email"] = ValidateStrictEmail,
            ["phonenumber"] = ValidatePhoneNumber,
            ["studentnumber"] = ValidateStudentNumber,
            ["coursecode"] = ValidateCourseCode,
            ["credithours"] = ValidateCreditHours,
            ["credits"] = ValidateCredits,
            ["birthdate"] = ValidateBirthDate,
            ["createdat"] = ValidateDateTime,
            ["updatedat"] = ValidateDateTime
        };
    }

    /// <summary>
    /// Inicializa validadores específicos por tipo de dato
    /// </summary>
    private Dictionary<Type, Func<object?, string, ValidationResult?>> InitializeTypeValidators()
    {
        return new Dictionary<Type, Func<object?, string, ValidationResult?>>
        {
            [typeof(int)] = ValidateInteger,
            [typeof(int?)] = ValidateNullableInteger,
            [typeof(bool)] = ValidateBoolean,
            [typeof(bool?)] = ValidateNullableBoolean,
            [typeof(DateTime)] = ValidateDateTime,
            [typeof(DateTime?)] = ValidateNullableDateTime
        };
    }

    /// <summary>
    /// Valida propiedades según el contexto inferido del nombre
    /// </summary>
    private ValidationResult? ValidateByPropertyContext(object? value, string propertyName, Type propertyType)
    {
        if (value == null) return null;

        var lowerName = propertyName.ToLowerInvariant();

        // Propiedades que deben contener solo texto alfabético
        if (IsTextProperty(lowerName))
        {
            return ValidateAlphabeticText(value, propertyName);
        }

        // Propiedades que deben ser códigos alfanuméricos
        if (IsCodeProperty(lowerName))
        {
            return ValidateAlphanumericCode(value, propertyName);
        }

        // Propiedades que deben ser descripciones (permiten más caracteres)
        if (IsDescriptionProperty(lowerName))
        {
            return ValidateDescriptionText(value, propertyName);
        }

        return null;
    }

    /// <summary>
    /// Determina si una propiedad debe contener solo texto alfabético
    /// </summary>
    private static bool IsTextProperty(string propertyName)
    {
        var textProperties = new[] { "name", "firstname", "lastname", "career", "department", "title" };
        return textProperties.Any(prop => propertyName.Contains(prop));
    }

    /// <summary>
    /// Determina si una propiedad debe ser un código alfanumérico
    /// </summary>
    private static bool IsCodeProperty(string propertyName)
    {
        var codeProperties = new[] { "code", "number", "id" };
        return codeProperties.Any(prop => propertyName.Contains(prop));
    }

    /// <summary>
    /// Determina si una propiedad es de descripción (permite más caracteres)
    /// </summary>
    private static bool IsDescriptionProperty(string propertyName)
    {
        var descriptionProperties = new[] { "description", "address", "prerequisites", "notes", "comments" };
        return descriptionProperties.Any(prop => propertyName.Contains(prop));
    }

    #region Validadores específicos

    private ValidationResult? ValidatePersonName(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var stringValue = value.ToString()!;

        if (IsNumericString(stringValue))
        {
            return new ValidationResult(
                $"The field '{propertyName}' cannot contain purely numeric values. Person names must contain alphabetic characters only.",
                new[] { propertyName });
        }

        if (!Regex.IsMatch(stringValue, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must contain only alphabetic characters and spaces. No numbers or special characters are allowed.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateCourseName(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var stringValue = value.ToString()!;

        if (IsNumericString(stringValue))
        {
            return new ValidationResult(
                $"The field '{propertyName}' cannot be purely numeric. Course names must contain descriptive text.",
                new[] { propertyName });
        }

        // Los nombres de cursos pueden contener números pero no ser puramente numéricos
        if (!Regex.IsMatch(stringValue, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-\.]+$"))
        {
            return new ValidationResult(
                $"The field '{propertyName}' contains invalid characters. Only letters, numbers, spaces, hyphens, and periods are allowed.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateCareerName(object? value, string propertyName)
    {
        return ValidatePersonName(value, propertyName); // Carreras usan las mismas reglas que nombres de personas
    }

    private ValidationResult? ValidateDepartmentName(object? value, string propertyName)
    {
        return ValidatePersonName(value, propertyName); // Departamentos usan las mismas reglas que nombres de personas
    }

    private ValidationResult? ValidateStrictEmail(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var emailValue = value.ToString()!;

        if (IsNumericString(emailValue))
        {
            return new ValidationResult(
                $"The field '{propertyName}' cannot be a numeric value. A valid email address is required.",
                new[] { propertyName });
        }

        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(emailValue, emailPattern))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be a valid email address format (e.g., user@domain.com).",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidatePhoneNumber(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var phoneValue = value.ToString()!;

        if (!Regex.IsMatch(phoneValue, @"^[\d\s\-\+\(\)]+$"))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must contain only numbers, spaces, hyphens, plus signs, and parentheses.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateStudentNumber(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var studentNumberValue = value.ToString()!;

        if (!Regex.IsMatch(studentNumberValue, @"^[A-Z0-9]+$"))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must contain only uppercase letters and numbers.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateCourseCode(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var codeValue = value.ToString()!;

        if (!Regex.IsMatch(codeValue, @"^[A-Z]{2,4}\d{3,4}$"))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must follow the format: 2-4 uppercase letters followed by 3-4 digits (e.g., COMP1234).",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateCreditHours(object? value, string propertyName)
    {
        if (value == null) return null;

        if (value.GetType() != typeof(int))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be an integer value, not a string conversion.",
                new[] { propertyName });
        }

        var intValue = (int)value;
        if (intValue < 1 || intValue > 6)
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be between 1 and 6 credit hours.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateCredits(object? value, string propertyName)
    {
        return ValidateCreditHours(value, propertyName); // Mismas reglas que credit hours
    }

    private ValidationResult? ValidateBirthDate(object? value, string propertyName)
    {
        if (value == null) return null;

        if (value.GetType() != typeof(DateTime) && value.GetType() != typeof(DateTime?))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be a DateTime object, not a string representation.",
                new[] { propertyName });
        }

        var dateValue = (DateTime)value;
        var currentDate = DateTime.Now;
        var minimumAge = currentDate.AddYears(-120); // Edad mínima razonable
        var maximumAge = currentDate.AddYears(-16);  // Edad máxima para estudiantes

        if (dateValue > maximumAge)
        {
            return new ValidationResult(
                $"The field '{propertyName}' indicates the person is too young. Minimum age requirement is 16 years.",
                new[] { propertyName });
        }

        if (dateValue < minimumAge)
        {
            return new ValidationResult(
                $"The field '{propertyName}' indicates an unrealistic age. Please verify the birth date.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateAlphabeticText(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var stringValue = value.ToString()!;

        if (IsNumericString(stringValue))
        {
            return new ValidationResult(
                $"The field '{propertyName}' cannot contain purely numeric values. Expected alphabetic text only.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateAlphanumericCode(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var stringValue = value.ToString()!;

        if (!Regex.IsMatch(stringValue, @"^[A-Z0-9]+$"))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must contain only uppercase letters and numbers.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateDescriptionText(object? value, string propertyName)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;

        var stringValue = value.ToString()!;

        if (IsNumericString(stringValue))
        {
            return new ValidationResult(
                $"The field '{propertyName}' cannot be purely numeric. Descriptions must contain descriptive text.",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateInteger(object? value, string propertyName)
    {
        if (value == null) return null;

        if (value.GetType() != typeof(int))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be an integer value, not a string conversion. Received type: {value.GetType().Name}",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateNullableInteger(object? value, string propertyName)
    {
        if (value == null) return null;
        return ValidateInteger(value, propertyName);
    }

    private ValidationResult? ValidateBoolean(object? value, string propertyName)
    {
        if (value == null) return null;

        if (value.GetType() != typeof(bool))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be a boolean value (true/false), not a string conversion. Received type: {value.GetType().Name}",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateNullableBoolean(object? value, string propertyName)
    {
        if (value == null) return null;
        return ValidateBoolean(value, propertyName);
    }

    private ValidationResult? ValidateDateTime(object? value, string propertyName)
    {
        if (value == null) return null;

        if (value.GetType() != typeof(DateTime) && value.GetType() != typeof(DateTime?))
        {
            return new ValidationResult(
                $"The field '{propertyName}' must be a DateTime object, not a string representation. Received type: {value.GetType().Name}",
                new[] { propertyName });
        }

        return null;
    }

    private ValidationResult? ValidateNullableDateTime(object? value, string propertyName)
    {
        if (value == null) return null;
        return ValidateDateTime(value, propertyName);
    }

    #endregion

    #region Métodos de utilidad

    private static bool IsNumericString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        // Verificar si es puramente numérico (incluyendo decimales)
        return Regex.IsMatch(value.Trim(), @"^\d+(\.\d+)?$") ||
               (decimal.TryParse(value, out _) && !value.Any(char.IsLetter));
    }

    #endregion
}
