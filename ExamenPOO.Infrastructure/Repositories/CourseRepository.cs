using Microsoft.EntityFrameworkCore;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Infrastructure.Data;

namespace ExamenPOO.Infrastructure.Repositories;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetByCourseCodeAsync(string courseCode)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
    }

    public async Task<bool> CourseCodeExistsAsync(string courseCode)
    {
        return await _dbSet.AnyAsync(c => c.CourseCode == courseCode);
    }

    public async Task<IEnumerable<Course>> GetByDepartmentAsync(string department)
    {
        return await _dbSet
            .Where(c => c.Department == department && c.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetByCreditRangeAsync(int minCredits, int maxCredits)
    {
        return await _dbSet
            .Where(c => c.Credits >= minCredits && c.Credits <= maxCredits && c.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(searchTerm) || 
                       (c.Description != null && c.Description.Contains(searchTerm)) ||
                       c.CourseCode.Contains(searchTerm))
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetAvailableCoursesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetByCreditHoursAsync(int creditHours)
    {
        return await _dbSet
            .Where(c => c.Credits == creditHours && c.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> SearchByNameOrCodeAsync(string searchTerm)
    {
        return await _dbSet
            .Where(c => (c.Name.Contains(searchTerm) || c.CourseCode.Contains(searchTerm)) && c.IsActive)
            .ToListAsync();
    }
}
