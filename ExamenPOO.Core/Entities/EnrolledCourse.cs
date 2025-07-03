using System.ComponentModel.DataAnnotations;

namespace ExamenPOO.Core.Entities;

public class EnrolledCourse
{
    public int Id { get; set; }
    
    [Required]
    public int SemesterEnrollmentId { get; set; }
    
    [Required]
    public int CourseId { get; set; }
    
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    
    [StringLength(2)]
    public string? Grade { get; set; } // Calificación final (A, B, C, D, F, null si no tiene calificación)
    
    public DateTime? GradeDate { get; set; } // Fecha cuando se asignó la calificación
    
    public bool IsDropped { get; set; } = false; // Si el estudiante abandonó la materia
    
    public DateTime? DropDate { get; set; } // Fecha cuando abandonó la materia
    
    // Navigation properties
    public virtual SemesterEnrollment SemesterEnrollment { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    
    // Computed properties
    public bool HasGrade => !string.IsNullOrEmpty(Grade);
    public bool IsCompleted => HasGrade && !IsDropped;
    public string Status => IsDropped ? "Abandonada" : HasGrade ? $"Completada ({Grade})" : "En Progreso";
    
    // Business logic methods
    public void AssignGrade(string grade)
    {
        if (IsDropped)
            throw new InvalidOperationException("No se puede asignar calificación a una materia abandonada");
        
        var validGrades = new[] { "A", "B", "C", "D", "F" };
        if (!validGrades.Contains(grade.ToUpper()))
            throw new ArgumentException("Calificación inválida. Las calificaciones válidas son: A, B, C, D, F");
        
        Grade = grade.ToUpper();
        GradeDate = DateTime.UtcNow;
    }
    
    public void DropCourse()
    {
        if (HasGrade)
            throw new InvalidOperationException("No se puede abandonar una materia que ya tiene calificación");
        
        IsDropped = true;
        DropDate = DateTime.UtcNow;
    }
}
