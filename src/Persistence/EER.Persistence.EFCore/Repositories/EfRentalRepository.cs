using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
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

    public async Task<IEnumerable<Rental>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Rental?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals.FindAsync([id], cancellationToken);
    }

    public async Task<Rental> AddAsync(Rental rental, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Rentals.AddAsync(rental, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId, Role userRole, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Rental> query = _context.Rentals.AsNoTracking();

        query = userRole switch
        {
            Role.Customer => query.Where(r => r.CustomerId == userId),
            Role.Owner => query.Where(r => r.OwnerId == userId),
            Role.Admin => query,
            _ => throw new ArgumentOutOfRangeException(nameof(userRole), $"Unsupported role: {userRole}")
        };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, Guid manipulator, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var id = rentalToUpdate.Id;

        var entity = await _context.Rentals.FindAsync([id], cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"entity with ID '{id}' was not found.");
        }

        entity.Status = rentalToUpdate.Status;
        entity.UpdatedBy = rentalToUpdate.UpdatedBy;
        entity.UpdatedAt = rentalToUpdate.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Rental?> GetByIdWithItemsAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals
            .AsNoTracking()
            .Include(r => r.RentalItems)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddRentalItemsAsync(IEnumerable<RentalItem> items, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await _context.RentalItems.AddRangeAsync(items, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Rentals.FindAsync([id], cancellationToken);
        if (existing is null) return false;

        _context.Rentals.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
