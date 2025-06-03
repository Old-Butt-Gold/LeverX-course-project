using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public EfRefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(Guid userId, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // to make all changes in one query
        await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ExecuteUpdateAsync(rt =>
                rt.SetProperty(pr => pr.RevokedAt, now), cancellationToken: cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token && rt.RevokedAt == null, cancellationToken);

        if (refreshToken != null)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
