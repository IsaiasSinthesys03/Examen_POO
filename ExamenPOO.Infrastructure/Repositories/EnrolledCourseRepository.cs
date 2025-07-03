using Microsoft.EntityFrameworkCore;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Infrastructure.Data;

namespace ExamenPOO.Infrastructure.Repositories;

public class EnrolledCourseRepository : Repository<EnrolledCourse>, IEnrolledCourseRepository
{
    public EnrolledCourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EnrolledCourse>> GetBySemesterEnrollmentIdAsync(int semesterEnrollmentId)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .Where(ec => ec.SemesterEnrollmentId == semesterEnrollmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EnrolledCourse>> GetByCourseIdAsync(int courseId)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .Where(ec => ec.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<EnrolledCourse?> GetByStudentAndCourseAsync(int studentId, int courseId)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .FirstOrDefaultAsync(ec => ec.SemesterEnrollment.StudentId == studentId && 
                               ec.CourseId == courseId);
    }

    public async Task<IEnumerable<EnrolledCourse>> GetByStudentIdAsync(int studentId)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .Where(ec => ec.SemesterEnrollment.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<EnrolledCourse?> GetWithCourseAsync(int enrolledCourseId)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .FirstOrDefaultAsync(ec => ec.Id == enrolledCourseId);
    }

    public async Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId)
    {
        return await _dbSet.AnyAsync(ec => ec.SemesterEnrollment.StudentId == studentId && 
                                         ec.CourseId == courseId);
    }

    public async Task<int> GetEnrollmentCountForCourseAsync(int courseId)
    {
        return await _dbSet.CountAsync(ec => ec.CourseId == courseId);
    }

    public override async Task<EnrolledCourse?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .FirstOrDefaultAsync(ec => ec.Id == id);
    }

    public override async Task<IEnumerable<EnrolledCourse>> GetAllAsync()
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .ToListAsync();
    }

    public override async Task<IEnumerable<EnrolledCourse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Include(ec => ec.SemesterEnrollment)
                .ThenInclude(se => se.Student)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
