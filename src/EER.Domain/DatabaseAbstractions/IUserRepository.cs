using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> UpdateAsync(User user, ITransaction? transaction = null, CancellationToken cancellationToken = default);

    Task<bool> IsEmailExists(string email, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
