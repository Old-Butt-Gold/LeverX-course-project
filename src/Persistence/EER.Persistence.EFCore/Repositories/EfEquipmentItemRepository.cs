using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfEquipmentItemRepository : IEquipmentItemRepository
{
    private readonly ApplicationDbContext _context;

    public EfEquipmentItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentItems.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentItems.FindAsync([id], cancellationToken);
    }

    public async Task<EquipmentItem> AddAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        var entry = await _context.EquipmentItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<EquipmentItem> UpdateAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        var entity = await _context.EquipmentItems.FindAsync([item.Id], cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"EquipmentItem with ID '{item.Id}' was not found.");
        }

        entity.OfficeId = item.OfficeId;
        entity.SerialNumber = item.SerialNumber;
        entity.ItemStatus = item.ItemStatus;
        entity.MaintenanceDate = item.MaintenanceDate;
        entity.PurchaseDate = item.PurchaseDate;
        entity.UpdatedBy = item.UpdatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.EquipmentItems.FindAsync([id], cancellationToken);
        if (existing is null) return false;

        _context.EquipmentItems.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
