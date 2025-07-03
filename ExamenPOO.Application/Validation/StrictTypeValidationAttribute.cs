using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ExamenPOO.Application.Validation;

/// <summary>
/// Atributo de validación estricta para text fields que deben contener solo caracteres alfabéticos
/// </summary>
public class StrictTextAttribute : ValidationAttribute
{
    private readonly bool _allowSpaces;
    private readonly bool _allowAccentedCharacters;

    public StrictTextAttribute(bool allowSpaces = true, bool allowAccentedCharacters = true)
    {
        _allowSpaces = allowSpaces;
        _allowAccentedCharacters = allowAccentedCharacters;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var stringValue = value.ToString()!;

        // Verificar que sea exactamente un string y no una conversión de número
        if (IsNumericString(stringValue))
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' cannot contain purely numeric values. Expected alphabetic text only.",
                new[] { validationContext.MemberName! });
        }

        // Patrón para caracteres alfabéticos
        string pattern = _allowAccentedCharacters 
            ? @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ" + (_allowSpaces ? @"\s" : "") + @"]+$"
            : @"^[a-zA-Z" + (_allowSpaces ? @"\s" : "") + @"]+$";

        if (!Regex.IsMatch(stringValue, pattern))
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must contain only alphabetic characters" + 
                (_allowSpaces ? " and spaces" : "") + ". No numbers or special characters are allowed.",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }

    private static bool IsNumericString(string value)
    {
        // Verificar si el string contiene solo números (posiblemente con espacios)
        return Regex.IsMatch(value.Trim(), @"^\d+$") || 
               (decimal.TryParse(value, out _) && !value.Any(char.IsLetter));
    }
}

/// <summary>
/// Atributo de validación estricta para códigos alfanuméricos con formato específico
/// </summary>
public class StrictCodeAttribute : ValidationAttribute
{
    private readonly string _pattern;
    private readonly string _description;

    public StrictCodeAttribute(string pattern, string description)
    {
        _pattern = pattern;
        _description = description;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var stringValue = value.ToString()!;

        if (!Regex.IsMatch(stringValue, _pattern))
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must match the format: {_description}. " +
                $"Received value '{stringValue}' does not match the required pattern.",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Atributo de validación estricta para números que deben ser valores numéricos auténticos
/// </summary>
public class StrictNumericAttribute : ValidationAttribute
{
    private readonly Type _expectedType;
    private readonly double _minimum;
    private readonly double _maximum;
    private readonly bool _hasMinimum;
    private readonly bool _hasMaximum;

    public StrictNumericAttribute(Type expectedType, double minimum = double.MinValue, double maximum = double.MaxValue)
    {
        _expectedType = expectedType;
        _minimum = minimum;
        _maximum = maximum;
        _hasMinimum = minimum != double.MinValue;
        _hasMaximum = maximum != double.MaxValue;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        // Verificar que el valor sea del tipo exacto esperado
        if (value.GetType() != _expectedType)
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must be of type {_expectedType.Name}. " +
                $"Received type {value.GetType().Name}. String-to-number conversions are not allowed.",
                new[] { validationContext.MemberName! });
        }

        // Validar rango si está especificado
        if (_hasMinimum || _hasMaximum)
        {
            var numericValue = Convert.ToDouble(value);
            
            if (_hasMinimum && numericValue < _minimum)
            {
                return new ValidationResult(
                    $"The field '{validationContext.DisplayName}' must be at least {_minimum}.",
                    new[] { validationContext.MemberName! });
            }

            if (_hasMaximum && numericValue > _maximum)
            {
                return new ValidationResult(
                    $"The field '{validationContext.DisplayName}' must not exceed {_maximum}.",
                    new[] { validationContext.MemberName! });
            }
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Atributo de validación estricta para fechas que deben ser objetos DateTime válidos
/// </summary>
public class StrictDateTimeAttribute : ValidationAttribute
{
    private readonly DateTime? _minimumDate;
    private readonly DateTime? _maximumDate;
    private readonly bool _allowFutureDates;

    public StrictDateTimeAttribute(bool allowFutureDates = true, string minimumDate = "", string maximumDate = "")
    {
        _allowFutureDates = allowFutureDates;
        
        if (!string.IsNullOrEmpty(minimumDate) && DateTime.TryParse(minimumDate, out var minDate))
            _minimumDate = minDate;
            
        if (!string.IsNullOrEmpty(maximumDate) && DateTime.TryParse(maximumDate, out var maxDate))
            _maximumDate = maxDate;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        // Verificar que sea un DateTime auténtico, no una conversión de string
        if (value.GetType() != typeof(DateTime) && value.GetType() != typeof(DateTime?))
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must be a valid DateTime object. " +
                $"String representations of dates are not allowed.",
                new[] { validationContext.MemberName! });
        }

        var dateValue = (DateTime)value;

        // Validar fechas futuras si no están permitidas
        if (!_allowFutureDates && dateValue > DateTime.Now)
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' cannot be a future date.",
                new[] { validationContext.MemberName! });
        }

        // Validar rango de fechas
        if (_minimumDate.HasValue && dateValue < _minimumDate.Value)
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must be after {_minimumDate.Value:yyyy-MM-dd}.",
                new[] { validationContext.MemberName! });
        }

        if (_maximumDate.HasValue && dateValue > _maximumDate.Value)
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must be before {_maximumDate.Value:yyyy-MM-dd}.",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Atributo de validación estricta para emails con formato y estructura específicos
/// </summary>
public class StrictEmailAttribute : ValidationAttribute
{
    private readonly string[] _allowedDomains;
    private readonly bool _requireSpecificDomains;

    public StrictEmailAttribute(bool requireSpecificDomains = false, params string[] allowedDomains)
    {
        _requireSpecificDomains = requireSpecificDomains;
        _allowedDomains = allowedDomains ?? Array.Empty<string>();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var emailValue = value.ToString()!;

        // Verificar que no sea un número convertido a string
        if (decimal.TryParse(emailValue, out _))
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' cannot be a numeric value. A valid email address is required.",
                new[] { validationContext.MemberName! });
        }

        // Validación estricta del formato de email
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(emailValue, emailPattern))
        {
            return new ValidationResult(
                $"The field '{validationContext.DisplayName}' must be a valid email address format. " +
                $"Example: user@domain.com",
                new[] { validationContext.MemberName! });
        }

        // Validar dominios específicos si es requerido
        if (_requireSpecificDomains && _allowedDomains.Length > 0)
        {
            var domain = emailValue.Split('@')[1].ToLowerInvariant();
            if (!_allowedDomains.Contains(domain))
            {
                return new ValidationResult(
                    $"The field '{validationContext.DisplayName}' must use an allowed domain. " +
                    $"Allowed domains: {string.Join(", ", _allowedDomains)}",
                    new[] { validationContext.MemberName! });
            }
        }

        return ValidationResult.Success;
    }
}
