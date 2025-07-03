using System.ComponentModel.DataAnnotations;
using ExamenPOO.Application.Validation;

namespace ExamenPOO.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // Additional Name property
    public string? Description { get; set; }
    public int CreditHours { get; set; }
    public int Credits { get; set; } // Additional Credits property
    public string Department { get; set; } = string.Empty;
    public string? Prerequisites { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string FullCourseInfo { get; set; } = string.Empty;
}

public class CreateCourseDto
{
    [Required]
    [StringLength(10)]
    [StrictCode(@"^[A-Z]{2,4}\d{3,4}$", "2-4 uppercase letters followed by 3-4 digits (e.g., COMP1234)")]
    public string CourseCode { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [StrictText]
    public string CourseName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [StrictText]
    public string Name { get; set; } = string.Empty; // Additional Name property
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StrictNumeric(typeof(int), 1, 6)]
    public int CreditHours { get; set; }
    
    [Required]
    [StrictNumeric(typeof(int), 1, 6)]
    public int Credits { get; set; } // Additional Credits property
    
    [Required]
    [StringLength(100)]
    [StrictText]
    public string Department { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Prerequisites { get; set; }
}

public class UpdateCourseDto
{
    [StringLength(10)]
    [StrictCode(@"^[A-Z]{2,4}\d{3,4}$", "2-4 uppercase letters followed by 3-4 digits (e.g., COMP1234)")]
    public string? CourseCode { get; set; }
    
    [StringLength(100)]
    [StrictText]
    public string? CourseName { get; set; }
    
    [StringLength(100)]
    [StrictText]
    public string? Name { get; set; } // Additional Name property
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StrictNumeric(typeof(int), 1, 6)]
    public int? CreditHours { get; set; }
    
    [StrictNumeric(typeof(int), 1, 6)]
    public int? Credits { get; set; } // Additional Credits property
    
    [StringLength(100)]
    [StrictText]
    public string? Department { get; set; }
    
    [StringLength(50)]
    public string? Prerequisites { get; set; }
    
    public bool? IsActive { get; set; }
}
