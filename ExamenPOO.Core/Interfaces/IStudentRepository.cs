using ExamenPOO.Core.Entities;

namespace ExamenPOO.Core.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByStudentNumberAsync(string studentNumber);
    Task<Student?> GetByEmailAsync(string email);
    Task<bool> StudentNumberExistsAsync(string studentNumber);
    Task<bool> EmailExistsAsync(string email);
    Task<Student?> GetWithEnrollmentsAsync(int studentId);
    Task<bool> HasActiveEnrollmentsAsync(int studentId);
    Task<IEnumerable<Student>> GetByCareerAsync(string career);
}
