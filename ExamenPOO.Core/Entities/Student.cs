using ExamenPOO.Core.ValueObjects;

namespace ExamenPOO.Core.Entities;

public class Student : BaseEntity
{
    public StudentNumber StudentNumber { get; internal set; } = null!;
    public Email Email { get; internal set; } = null!;
    public string PasswordHash { get; internal set; } = string.Empty;
    public string FirstName { get; internal set; } = string.Empty;
    public string LastName { get; internal set; } = string.Empty;
    public string? PhoneNumber { get; internal set; }
    public string? Address { get; internal set; }
    public DateTime BirthDate { get; internal set; }
    public bool IsActive { get; internal set; }
    public string Career { get; internal set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<SemesterEnrollment> SemesterEnrollments { get; private set; } = new List<SemesterEnrollment>();
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    // Private constructor for EF
    internal Student() { }

    // Public constructor with business logic
    public Student(StudentNumber studentNumber, Email email, string firstName, string lastName, 
                   DateTime birthDate, string career, string? phoneNumber = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));
        
        if (string.IsNullOrWhiteSpace(career))
            throw new ArgumentException("Career is required", nameof(career));

        StudentNumber = studentNumber;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Career = career;
        PhoneNumber = phoneNumber;
        Address = address;
        IsActive = true;
        PasswordHash = string.Empty; // Will be set separately
    }

    // Business methods
    public void SetPassword(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Password hash cannot be empty", nameof(hashedPassword));
        
        PasswordHash = hashedPassword;
        SetUpdatedAt();
    }

    public void UpdatePersonalInfo(string firstName, string lastName, string? phoneNumber, string? address)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Address = address;
        SetUpdatedAt();
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }
    
    // Business logic method
    public bool HasActiveEnrollments()
    {
        return SemesterEnrollments.Any(se => se.IsActive);
    }

    // Factory method for EF Core seeding
    public static Student CreateForSeed(int id, string studentNumber, string email, string firstName, 
                                       string lastName, string career, DateTime birthDate, string passwordHash)
    {
        var student = new Student();
        
        // Use reflection to set private setters for seeding
        typeof(BaseEntity).GetProperty(nameof(Id))!.SetValue(student, id);
        typeof(BaseEntity).GetProperty(nameof(CreatedAt))!.SetValue(student, DateTime.UtcNow);
        
        student.StudentNumber = new StudentNumber(studentNumber);
        student.Email = new Email(email);
        student.FirstName = firstName;
        student.LastName = lastName;
        student.Career = career;
        student.BirthDate = birthDate;
        student.PasswordHash = passwordHash;
        student.IsActive = true;
        
        return student;
    }
}
