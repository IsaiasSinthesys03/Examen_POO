using Microsoft.EntityFrameworkCore;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Infrastructure.Data;

namespace ExamenPOO.Infrastructure.Repositories;

public class SemesterEnrollmentRepository : Repository<SemesterEnrollment>, ISemesterEnrollmentRepository
{
    public SemesterEnrollmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SemesterEnrollment?> GetByStudentAndSemesterAsync(int studentId, string semester, int year)
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .FirstOrDefaultAsync(se => se.StudentId == studentId && 
                               se.Semester == semester && 
                               se.Year == year);
    }

    public async Task<bool> ExistsForStudentAndSemesterAsync(int studentId, string semester, int year)
    {
        return await _dbSet.AnyAsync(se => se.StudentId == studentId && 
                                         se.Semester == semester && 
                                         se.Year == year);
    }

    public async Task<IEnumerable<SemesterEnrollment>> GetByStudentIdAsync(int studentId)
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .Where(se => se.StudentId == studentId)
            .OrderByDescending(se => se.Year)
            .ThenByDescending(se => se.Semester)
            .ToListAsync();
    }

    public async Task<IEnumerable<SemesterEnrollment>> GetBySemesterAsync(string semester, int year)
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .Where(se => se.Semester == semester && se.Year == year)
            .ToListAsync();
    }

    public async Task<SemesterEnrollment?> GetWithEnrolledCoursesAsync(int semesterEnrollmentId)
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .FirstOrDefaultAsync(se => se.Id == semesterEnrollmentId);
    }

    public async Task<int> GetTotalCreditsForSemesterAsync(int semesterEnrollmentId)
    {
        var semesterEnrollment = await _dbSet
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .FirstOrDefaultAsync(se => se.Id == semesterEnrollmentId);

        return semesterEnrollment?.GetTotalCredits() ?? 0;
    }

    public async Task<SemesterEnrollment?> GetWithCoursesAsync(int semesterEnrollmentId)
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .FirstOrDefaultAsync(se => se.Id == semesterEnrollmentId);
    }

    public async Task<IEnumerable<SemesterEnrollment>> GetActiveSemestersAsync()
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.EnrolledCourses)
                .ThenInclude(ec => ec.Course)
            .Where(se => se.IsActive)
            .ToListAsync();
    }

    public async Task<bool> HasActiveEnrollmentAsync(int studentId)
    {
        return await _dbSet.AnyAsync(se => se.StudentId == studentId && se.IsActive);
    }
}
