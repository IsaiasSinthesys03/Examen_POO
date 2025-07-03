using System.Text.RegularExpressions;

namespace ExamenPOO.Core.ValueObjects;

public class StudentNumber : IEquatable<StudentNumber>
{
    private static readonly Regex StudentNumberRegex = new(
        @"^[A-Z0-9]{6,20}$",
        RegexOptions.Compiled);

    public string Value { get; }

    public StudentNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Student number cannot be empty", nameof(value));

        var normalizedValue = value.ToUpperInvariant().Trim();

        if (!StudentNumberRegex.IsMatch(normalizedValue))
            throw new ArgumentException(
                "Student number must contain only letters and numbers and be between 6-20 characters", 
                nameof(value));

        Value = normalizedValue;
    }

    public bool Equals(StudentNumber? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is StudentNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(StudentNumber studentNumber)
    {
        return studentNumber.Value;
    }

    public static explicit operator StudentNumber(string studentNumber)
    {
        return new StudentNumber(studentNumber);
    }
}
