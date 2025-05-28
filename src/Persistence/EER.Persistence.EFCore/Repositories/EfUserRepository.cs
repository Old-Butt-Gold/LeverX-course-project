using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public EfUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([id], cancellationToken: cancellationToken);

        return user;
    }

    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Users.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FindAsync([user.Id], cancellationToken);

        if (entity is null) return null;

        entity.Email = user.Email;
        entity.PasswordHash = user.PasswordHash;
        entity.FullName = user.FullName;
        entity.UserRole = user.UserRole;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FindAsync([id], cancellationToken);

        if (entity is null) return false;

        _context.Users.Remove(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
