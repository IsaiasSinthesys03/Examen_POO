using ExamenPOO.Core.Entities;

namespace ExamenPOO.Core.Interfaces;

public interface IEnrolledCourseRepository : IRepository<EnrolledCourse>
{
    Task<IEnumerable<EnrolledCourse>> GetBySemesterEnrollmentIdAsync(int semesterEnrollmentId);
    Task<EnrolledCourse?> GetByStudentAndCourseAsync(int studentId, int courseId);
    Task<IEnumerable<EnrolledCourse>> GetByStudentIdAsync(int studentId);
    Task<IEnumerable<EnrolledCourse>> GetByCourseIdAsync(int courseId);
    Task<EnrolledCourse?> GetWithCourseAsync(int enrolledCourseId);
    Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId);
    Task<int> GetEnrollmentCountForCourseAsync(int courseId);
}
