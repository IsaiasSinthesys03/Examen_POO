using System.ComponentModel.DataAnnotations;

namespace ExamenPOO.Core.Entities;

public class SemesterEnrollment
{
    public int Id { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty; // Semester property
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    [Range(12, 30)]
    public int MaxCredits { get; set; } = 21; // MaxCredits property
    
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow; // Enrollment date
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<EnrolledCourse> EnrolledCourses { get; set; } = new List<EnrolledCourse>();
    
    // Computed properties
    public int CurrentCreditHours => EnrolledCourses.Sum(ec => ec.Course.CreditHours);
    public int AvailableCreditHours => MaxCredits - CurrentCreditHours;
    public string SemesterFullName => $"{Semester} {Year}";
    
    // Business logic methods
    public bool CanEnrollCourse(int creditHours)
    {
        if (!IsActive)
            return false;
            
        return (CurrentCreditHours + creditHours) <= MaxCredits;
    }
    
    public void EnrollCourse(Course course)
    {
        if (!CanEnrollCourse(course.CreditHours))
            throw new InvalidOperationException(
                $"No se puede inscribir la materia {course.CourseName}. " +
                $"Créditos actuales: {CurrentCreditHours}, " +
                $"Créditos de la materia: {course.CreditHours}, " +
                $"Límite máximo: {MaxCredits}");
        
        // Verificar si ya está inscrita
        if (EnrolledCourses.Any(ec => ec.CourseId == course.Id))
            throw new InvalidOperationException($"El estudiante ya está inscrito en la materia {course.CourseName}");
        
        var enrolledCourse = new EnrolledCourse
        {
            SemesterEnrollmentId = Id,
            CourseId = course.Id,
            EnrollmentDate = DateTime.UtcNow
        };
        
        EnrolledCourses.Add(enrolledCourse);
    }
    
    public void UnenrollCourse(int courseId)
    {
        var enrolledCourse = EnrolledCourses.FirstOrDefault(ec => ec.CourseId == courseId);
        if (enrolledCourse == null)
            throw new KeyNotFoundException("La materia no está inscrita en este semestre");
        
        EnrolledCourses.Remove(enrolledCourse);
    }
    
    public bool IsEnrolledInCourse(int courseId)
    {
        return EnrolledCourses.Any(ec => ec.CourseId == courseId);
    }
    
    public int GetTotalCredits()
    {
        return EnrolledCourses.Sum(ec => ec.Course.Credits);
    }
}
