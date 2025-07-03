using System.Linq.Expressions;

namespace ExamenPOO.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> GetCountAsync();
    Task<int> CountAsync(); // Additional count method
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> Update(T entity); // Additional update method
    Task<bool> DeleteAsync(int id);
    Task<bool> Delete(T entity); // Additional delete method
    Task<bool> ExistsAsync(int id);
}
