using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
}
