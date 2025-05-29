using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfOfficeRepository : IOfficeRepository
{
    private readonly ApplicationDbContext _context;

    public EfOfficeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Offices.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Offices.FindAsync([id], cancellationToken);
    }

    public async Task<Office> AddAsync(Office office, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Offices.AddAsync(office, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<Office> UpdateAsync(Office office, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Offices.FindAsync([office.Id], cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"Office with ID '{office.Id}' was not found.");
        }

        entity.Address = office.Address;
        entity.City = office.City;
        entity.Country = office.Country;
        entity.IsActive = office.IsActive;
        entity.UpdatedBy = office.UpdatedBy;
        entity.UpdatedAt = office.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Offices.FindAsync([id], cancellationToken);
        if (existing is null) return false;

        _context.Offices.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
