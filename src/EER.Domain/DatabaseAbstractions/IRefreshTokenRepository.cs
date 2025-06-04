using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(Guid userId, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
