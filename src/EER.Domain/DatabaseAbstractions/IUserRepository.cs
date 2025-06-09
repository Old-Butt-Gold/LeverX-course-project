using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> UpdateAsync(User user, ITransaction? transaction = null, CancellationToken cancellationToken = default);

    Task<bool> IsEmailExistsAsync(string email, Guid? excludeUserId = null, ITransaction? transaction = null, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, ITransaction? transaction = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
