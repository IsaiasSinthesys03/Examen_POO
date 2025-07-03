using ExamenPOO.Core.Entities;

namespace ExamenPOO.Core.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetByDepartmentAsync(string department);
    Task<IEnumerable<Course>> GetByCreditHoursAsync(int creditHours);
    Task<IEnumerable<Course>> SearchByNameOrCodeAsync(string searchTerm);
    Task<Course?> GetByCourseCodeAsync(string courseCode);
    Task<bool> CourseCodeExistsAsync(string courseCode);
    Task<IEnumerable<Course>> GetByCreditRangeAsync(int minCredits, int maxCredits);
    Task<IEnumerable<Course>> SearchByNameAsync(string name);
    Task<IEnumerable<Course>> GetAvailableCoursesAsync();
}
