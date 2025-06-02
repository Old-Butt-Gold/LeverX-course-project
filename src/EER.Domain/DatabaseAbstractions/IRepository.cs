using EER.Domain.DatabaseAbstractions.Transaction;

namespace EER.Domain.DatabaseAbstractions;

public interface IRepository<T, in TKey> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(TKey id, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T item, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TKey id, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
