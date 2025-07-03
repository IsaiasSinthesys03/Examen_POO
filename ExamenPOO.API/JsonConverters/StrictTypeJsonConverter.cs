using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExamenPOO.API.JsonConverters;

/// <summary>
/// Conversor JSON estricto que previene conversiones implícitas de tipos específicos
/// </summary>
public class StrictStringJsonConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"Expected string but found {reader.TokenType}. " +
                $"Automatic type conversions are not allowed in strict mode. " +
                $"If you meant to send a string, please enclose the value in quotes.");
        }

        var stringValue = reader.GetString() ?? string.Empty;

        // Verificaciones adicionales para strings que podrían ser valores convertidos
        if (IsNumericString(stringValue))
        {
            throw new JsonException(
                $"String value '{stringValue}' appears to be purely numeric. " +
                $"If this is intended as a number, send it without quotes. " +
                $"If this is intended as text, ensure it contains non-numeric characters.");
        }

        return stringValue;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }

    private static bool IsNumericString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return decimal.TryParse(value, out _) && !value.Any(char.IsLetter);
    }
}

/// <summary>
/// Conversor JSON estricto para enteros
/// </summary>
public class StrictInt32JsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            throw new JsonException(
                $"Expected number but found string '{stringValue}'. " +
                $"Automatic string-to-number conversions are not allowed. " +
                $"Please send numeric values without quotes.");
        }

        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException(
                $"Expected number but found {reader.TokenType}. " +
                $"Please send an integer value.");
        }

        if (!reader.TryGetInt32(out var value))
        {
            throw new JsonException(
                $"Number value is outside the valid range for Int32 or contains decimal places. " +
                $"Please send a valid integer between {int.MinValue} and {int.MaxValue}.");
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

/// <summary>
/// Conversor JSON estricto para enteros nullable
/// </summary>
public class StrictNullableInt32JsonConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            throw new JsonException(
                $"Expected number but found string '{stringValue}'. " +
                $"Automatic string-to-number conversions are not allowed. " +
                $"Please send numeric values without quotes.");
        }

        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException(
                $"Expected number but found {reader.TokenType}. " +
                $"Please send an integer value.");
        }

        if (!reader.TryGetInt32(out var value))
        {
            throw new JsonException(
                $"Number value is outside the valid range for Int32 or contains decimal places. " +
                $"Please send a valid integer between {int.MinValue} and {int.MaxValue}.");
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

/// <summary>
/// Conversor JSON estricto para booleanos
/// </summary>
public class StrictBooleanJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            throw new JsonException(
                $"Expected boolean but found string '{stringValue}'. " +
                $"Automatic string-to-boolean conversions are not allowed. " +
                $"Please send true or false without quotes.");
        }

        if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
        {
            throw new JsonException(
                $"Expected boolean (true/false) but found {reader.TokenType}. " +
                $"Please send a boolean value.");
        }

        return reader.GetBoolean();
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}

/// <summary>
/// Conversor JSON estricto para booleanos nullable
/// </summary>
public class StrictNullableBooleanJsonConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            throw new JsonException(
                $"Expected boolean but found string '{stringValue}'. " +
                $"Automatic string-to-boolean conversions are not allowed. " +
                $"Please send true or false without quotes.");
        }

        if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
        {
            throw new JsonException(
                $"Expected boolean (true/false) but found {reader.TokenType}. " +
                $"Please send a boolean value.");
        }

        return reader.GetBoolean();
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

/// <summary>
/// Conversor JSON estricto para DateTime
/// </summary>
public class StrictDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"Expected DateTime string but found {reader.TokenType}. " +
                $"DateTime values must be sent as ISO 8601 formatted strings.");
        }

        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            throw new JsonException("DateTime string cannot be null or empty.");
        }

        if (!DateTime.TryParse(stringValue, out var dateValue))
        {
            throw new JsonException(
                $"Invalid DateTime string format: '{stringValue}'. " +
                $"Please use ISO 8601 format (yyyy-MM-ddTHH:mm:ss.fffZ).");
        }

        return dateValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}

/// <summary>
/// Conversor JSON estricto para DateTime nullable
/// </summary>
public class StrictNullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"Expected DateTime string but found {reader.TokenType}. " +
                $"DateTime values must be sent as ISO 8601 formatted strings.");
        }

        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            throw new JsonException("DateTime string cannot be null or empty.");
        }

        if (!DateTime.TryParse(stringValue, out var dateValue))
        {
            throw new JsonException(
                $"Invalid DateTime string format: '{stringValue}'. " +
                $"Please use ISO 8601 format (yyyy-MM-ddTHH:mm:ss.fffZ).");
        }

        return dateValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        else
            writer.WriteNullValue();
    }
}

/// <summary>
/// Conversor JSON estricto para decimales
/// </summary>
public class StrictDecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            throw new JsonException(
                $"Expected number but found string '{stringValue}'. " +
                $"Automatic string-to-number conversions are not allowed. " +
                $"Please send numeric values without quotes.");
        }

        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException(
                $"Expected number but found {reader.TokenType}. " +
                $"Please send a decimal value.");
        }

        if (!reader.TryGetDecimal(out var value))
        {
            throw new JsonException(
                $"Number value is outside the valid range for Decimal. " +
                $"Please send a valid decimal value.");
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

/// <summary>
/// Conversor JSON estricto para decimales nullable
/// </summary>
public class StrictNullableDecimalJsonConverter : JsonConverter<decimal?>
{
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            throw new JsonException(
                $"Expected number but found string '{stringValue}'. " +
                $"Automatic string-to-number conversions are not allowed. " +
                $"Please send numeric values without quotes.");
        }

        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException(
                $"Expected number but found {reader.TokenType}. " +
                $"Please send a decimal value.");
        }

        if (!reader.TryGetDecimal(out var value))
        {
            throw new JsonException(
                $"Number value is outside the valid range for Decimal. " +
                $"Please send a valid decimal value.");
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

/// <summary>
/// Factory para crear conversores JSON estrictos
/// </summary>
public class StrictTypeJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string) ||
               typeToConvert == typeof(int) ||
               typeToConvert == typeof(int?) ||
               typeToConvert == typeof(bool) ||
               typeToConvert == typeof(bool?) ||
               typeToConvert == typeof(DateTime) ||
               typeToConvert == typeof(DateTime?) ||
               typeToConvert == typeof(decimal) ||
               typeToConvert == typeof(decimal?);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return typeToConvert switch
        {
            Type t when t == typeof(string) => new StrictStringJsonConverter(),
            Type t when t == typeof(int) => new StrictInt32JsonConverter(),
            Type t when t == typeof(int?) => new StrictNullableInt32JsonConverter(),
            Type t when t == typeof(bool) => new StrictBooleanJsonConverter(),
            Type t when t == typeof(bool?) => new StrictNullableBooleanJsonConverter(),
            Type t when t == typeof(DateTime) => new StrictDateTimeJsonConverter(),
            Type t when t == typeof(DateTime?) => new StrictNullableDateTimeJsonConverter(),
            Type t when t == typeof(decimal) => new StrictDecimalJsonConverter(),
            Type t when t == typeof(decimal?) => new StrictNullableDecimalJsonConverter(),
            _ => throw new NotSupportedException($"Conversion of type {typeToConvert} is not supported by StrictTypeJsonConverterFactory.")
        };
    }
}
