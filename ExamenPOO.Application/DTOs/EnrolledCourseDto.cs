using System.ComponentModel.DataAnnotations;

namespace ExamenPOO.Application.DTOs;

public class EnrolledCourseDto
{
    public int Id { get; set; }
    public int SemesterEnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty; // CourseCode property
    public string CourseName { get; set; } = string.Empty; // CourseName property
    public int Credits { get; set; } // Credits property
    public string Department { get; set; } = string.Empty; // Department property
    public DateTime EnrollmentDate { get; set; }
    public string? Grade { get; set; }
    public DateTime? GradeDate { get; set; }
    public bool IsDropped { get; set; }
    public DateTime? DropDate { get; set; }
    public bool HasGrade { get; set; }
    public bool IsCompleted { get; set; }
    public string Status { get; set; } = string.Empty;
    public CourseDto Course { get; set; } = null!;
}

public class CreateEnrolledCourseDto
{
    [Required]
    public int SemesterEnrollmentId { get; set; }
    
    [Required]
    public int CourseId { get; set; }
}

public class EnrollCourseDto
{
    [Required]
    public int CourseId { get; set; }
}

public class AssignGradeDto
{
    [Required]
    [RegularExpression("^[ABCDF]$", ErrorMessage = "Grade must be A, B, C, D, or F")]
    public string Grade { get; set; } = string.Empty;
}
