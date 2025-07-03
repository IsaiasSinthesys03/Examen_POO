using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamenPOO.Core.Entities;

public class Course
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(10)]
    public string CourseCode { get; set; } = string.Empty; // Ej: MAT101, PRG101
    
    [Required]
    [StringLength(100)]
    public string CourseName { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(1, 6)]
    public int CreditHours { get; set; } // Número de créditos (1-6)
    
    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty; // Departamento académico
    
    [StringLength(50)]
    public string? Prerequisites { get; set; } // Prerrequisitos
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<EnrolledCourse> EnrolledCourses { get; set; } = new List<EnrolledCourse>();
    
    // Computed property
    public string FullCourseInfo => $"{CourseCode} - {CourseName} ({CreditHours} créditos)";
    
    // Additional properties
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // Course name property
    
    [Required]
    [Range(1, 6)]
    public int Credits { get; set; } // Credits property (alias for CreditHours)
}
