namespace ExamenPOO.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    ICourseRepository Courses { get; }
    ISemesterEnrollmentRepository SemesterEnrollments { get; }
    IEnrolledCourseRepository EnrolledCourses { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
