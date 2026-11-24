namespace LW3.Repositories;

public interface IRepository<T>
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task<T> CreateAsync(T model);
    Task<bool> UpdateAsync(string id, T model);
    Task<bool> DeleteAsync(string id);
}
