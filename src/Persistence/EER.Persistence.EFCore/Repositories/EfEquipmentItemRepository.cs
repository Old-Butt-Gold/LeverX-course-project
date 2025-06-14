﻿using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfEquipmentItemRepository : IEquipmentItemRepository
{
    private readonly ApplicationDbContext _context;

    public EfEquipmentItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentItems.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentItems.FindAsync([id], cancellationToken);
    }

    public async Task<EquipmentItem> AddAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entry = await _context.EquipmentItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<EquipmentItem> UpdateAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken cancellationToken = default)
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
        entity.UpdatedAt = item.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<EquipmentItem>> GetByIdsWithEquipmentAsync(IEnumerable<long> ids, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentItems
            .AsNoTracking()
            .Include(ei => ei.Equipment)
            .Where(ei => ids.Contains(ei.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStatusForItemsAsync(IEnumerable<long> itemIds, ItemStatus status, Guid updatedBy, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var ids = itemIds.ToList();
        var now = DateTime.UtcNow;

        await _context.EquipmentItems
            .Where(ei => ids.Contains(ei.Id))
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(ei => ei.ItemStatus, status)
                    .SetProperty(ei => ei.UpdatedBy, updatedBy)
                    .SetProperty(ei => ei.UpdatedAt, now),
                cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var existing = await _context.EquipmentItems.FindAsync([id], cancellationToken);
        if (existing is null) return false;

        _context.EquipmentItems.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
