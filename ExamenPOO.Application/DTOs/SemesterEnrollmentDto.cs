using System.ComponentModel.DataAnnotations;

namespace ExamenPOO.Application.DTOs;

public class SemesterEnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty; // StudentNumber property
    public string SemesterName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty; // Semester property
    public int Year { get; set; }
    public int MaxCreditHours { get; set; }
    public int MaxCredits { get; set; } // MaxCredits property
    public int TotalCredits { get; set; } // TotalCredits property
    public DateTime EnrollmentDate { get; set; } // EnrollmentDate property
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CurrentCreditHours { get; set; }
    public int AvailableCreditHours { get; set; }
    public string SemesterFullName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public List<EnrolledCourseDto> EnrolledCourses { get; set; } = new();
}

public class CreateSemesterEnrollmentDto
{
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string SemesterName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty; // Semester property
    
    [Required]
    [Range(2020, 2030)]
    public int Year { get; set; }
    
    [Required]
    [Range(12, 30)]
    public int MaxCreditHours { get; set; } = 21;
    
    [Required]
    [Range(12, 30)]
    public int MaxCredits { get; set; } = 21; // MaxCredits property
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
}

public class UpdateSemesterEnrollmentDto
{
    [StringLength(50)]
    public string? SemesterName { get; set; }
    
    [Range(2020, 2030)]
    public int? Year { get; set; }
    
    [Range(12, 30)]
    public int? MaxCreditHours { get; set; }
    
    [Range(12, 30)]
    public int? MaxCredits { get; set; } // MaxCredits property
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public bool? IsActive { get; set; }
}
