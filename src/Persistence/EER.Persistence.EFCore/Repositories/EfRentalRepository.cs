using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfRentalRepository : IRentalRepository
{
    private readonly ApplicationDbContext _context;

    public EfRentalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Rentals.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals.FindAsync([id], cancellationToken);
    }

    public async Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Rentals.AddAsync(rental, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<Rental?> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Rentals.FindAsync([id], cancellationToken);
        if (existing is null) return null;

        existing.Status = status;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Rentals.FindAsync([id], cancellationToken);
        if (existing is null) return false;

        _context.Rentals.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
