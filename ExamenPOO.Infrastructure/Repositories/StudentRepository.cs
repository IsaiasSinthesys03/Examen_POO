using Microsoft.EntityFrameworkCore;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Infrastructure.Data;

namespace ExamenPOO.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByStudentNumberAsync(string studentNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<bool> StudentNumberExistsAsync(string studentNumber)
    {
        return await _dbSet.AnyAsync(s => s.StudentNumber == studentNumber);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Student>> GetByCareerAsync(string career)
    {
        return await _dbSet.Where(s => s.Career == career && s.IsActive).ToListAsync();
    }

    public async Task<Student?> GetWithEnrollmentsAsync(int studentId)
    {
        return await _dbSet
            .Include(s => s.SemesterEnrollments)
                .ThenInclude(se => se.EnrolledCourses)
                    .ThenInclude(ec => ec.Course)
            .FirstOrDefaultAsync(s => s.Id == studentId);
    }

    public async Task<bool> HasActiveEnrollmentsAsync(int studentId)
    {
        return await _dbSet
            .Where(s => s.Id == studentId)
            .SelectMany(s => s.SemesterEnrollments)
            .AnyAsync(se => se.IsActive);
    }
}
