using System.ComponentModel.DataAnnotations;
using ExamenPOO.Application.Validation;

namespace ExamenPOO.Application.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Career { get; set; } = string.Empty; // Career property
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class CreateStudentDto
{
    [Required]
    [StringLength(20)]
    [StrictCode(@"^[A-Z0-9]{6,20}$", "6-20 uppercase letters and numbers (e.g., STUD123456)")]
    public string StudentNumber { get; set; } = string.Empty;
    
    [Required]
    [StrictEmail]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [StrictText(allowSpaces: false)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [StrictText(allowSpaces: false)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(15)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    [StrictDateTime(allowFutureDates: false, minimumDate: "1920-01-01")]
    public DateTime? BirthDate { get; set; }
    
    [Required]
    [StringLength(100)]
    [StrictText]
    public string Career { get; set; } = string.Empty;
}

public class UpdateStudentDto
{
    [StringLength(100)]
    [StrictEmail]
    public string? Email { get; set; }
    
    [StringLength(100)]
    [StrictText(allowSpaces: false)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    [StrictText(allowSpaces: false)]
    public string? LastName { get; set; }
    
    [StringLength(15)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    [StrictDateTime(allowFutureDates: false, minimumDate: "1920-01-01")]
    public DateTime? BirthDate { get; set; }
    
    [StringLength(100)]
    [StrictText]
    public string? Career { get; set; }
    
    public bool? IsActive { get; set; }
}

public class LoginResponseDto
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Career { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public StudentDto Student { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
