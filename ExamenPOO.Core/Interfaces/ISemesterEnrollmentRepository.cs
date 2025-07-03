using ExamenPOO.Core.Entities;

namespace ExamenPOO.Core.Interfaces;

public interface ISemesterEnrollmentRepository : IRepository<SemesterEnrollment>
{
    Task<IEnumerable<SemesterEnrollment>> GetByStudentIdAsync(int studentId);
    Task<SemesterEnrollment?> GetByStudentAndSemesterAsync(int studentId, string semesterName, int year);
    Task<SemesterEnrollment?> GetWithCoursesAsync(int semesterEnrollmentId);
    Task<IEnumerable<SemesterEnrollment>> GetActiveSemestersAsync();
    Task<bool> HasActiveEnrollmentAsync(int studentId);
    Task<IEnumerable<SemesterEnrollment>> GetBySemesterAsync(string semesterName, int year);
    Task<SemesterEnrollment?> GetWithEnrolledCoursesAsync(int semesterEnrollmentId);
    Task<bool> ExistsForStudentAndSemesterAsync(int studentId, string semester, int year);
    Task<int> GetTotalCreditsForSemesterAsync(int semesterEnrollmentId);
}
