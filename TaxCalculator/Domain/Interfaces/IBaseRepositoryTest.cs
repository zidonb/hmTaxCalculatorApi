namespace TaxCalculator.Domain.Interfaces;

// T - Is model from db.

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseRepositoryTest<T> where T : class {
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
