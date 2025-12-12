using System.Linq.Expressions;

namespace Library.Core.Interfaces;

public interface IRepository<T> where T : class
{
    // Get operations
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    // Add operations
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    // Update operations
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);

    // Delete operations
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // Count operations
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    // Exists operations
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
