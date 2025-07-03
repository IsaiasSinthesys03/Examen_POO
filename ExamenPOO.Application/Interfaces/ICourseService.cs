using ExamenPOO.Application.DTOs;

namespace ExamenPOO.Application.Interfaces;

public interface ICourseService
{
    Task<PagedResultDto<CourseDto>> GetCoursesAsync(int pageNumber = 1, int pageSize = 10);
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CourseDto?> GetCourseByCourseCodeAsync(string courseCode);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
    Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto updateCourseDto);
    Task<bool> DeleteCourseAsync(int id);
    Task<bool> CourseCodeExistsAsync(string courseCode);
    Task<IEnumerable<CourseDto>> GetByDepartmentAsync(string department);
    Task<IEnumerable<CourseDto>> GetByCreditRangeAsync(int minCredits, int maxCredits);
    Task<IEnumerable<CourseDto>> SearchByNameAsync(string searchTerm);
    Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync();
}
