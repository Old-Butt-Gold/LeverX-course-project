namespace EER.Domain.DatabaseAbstractions;

public interface IRepository<T, in TKey> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}
