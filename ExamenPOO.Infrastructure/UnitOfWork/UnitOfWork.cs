using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Infrastructure.Data;
using ExamenPOO.Infrastructure.Repositories;

namespace ExamenPOO.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Students = new StudentRepository(_context);
        Courses = new CourseRepository(_context);
        SemesterEnrollments = new SemesterEnrollmentRepository(_context);
        EnrolledCourses = new EnrolledCourseRepository(_context);
    }

    public IStudentRepository Students { get; private set; }
    public ICourseRepository Courses { get; private set; }
    public ISemesterEnrollmentRepository SemesterEnrollments { get; private set; }
    public IEnrolledCourseRepository EnrolledCourses { get; private set; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
