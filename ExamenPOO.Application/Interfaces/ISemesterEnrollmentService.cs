using ExamenPOO.Application.DTOs;

namespace ExamenPOO.Application.Interfaces;

public interface ISemesterEnrollmentService
{
    Task<PagedResultDto<SemesterEnrollmentDto>> GetSemesterEnrollmentsAsync(int pageNumber, int pageSize);
    Task<SemesterEnrollmentDto?> GetSemesterEnrollmentByIdAsync(int id);
    Task<PagedResultDto<SemesterEnrollmentDto>> GetSemesterEnrollmentsByStudentIdAsync(int studentId, int pageNumber, int pageSize);
    Task<SemesterEnrollmentDto> CreateSemesterEnrollmentAsync(CreateSemesterEnrollmentDto createSemesterEnrollmentDto);
    Task<SemesterEnrollmentDto> UpdateSemesterEnrollmentAsync(int id, UpdateSemesterEnrollmentDto updateSemesterEnrollmentDto);
    Task<bool> DeleteSemesterEnrollmentAsync(int id);
    Task<EnrolledCourseDto> EnrollCourseAsync(int semesterEnrollmentId, EnrollCourseDto enrollCourseDto);
    Task<bool> UnenrollCourseAsync(int semesterEnrollmentId, int courseId);
    Task<PagedResultDto<EnrolledCourseDto>> GetEnrolledCoursesAsync(int semesterEnrollmentId, int pageNumber, int pageSize);
}
