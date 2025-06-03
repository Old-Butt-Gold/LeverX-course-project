using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

internal sealed class EfEquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EfEquipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Equipment.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Equipment?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Equipment.FindAsync([id], cancellationToken);
    }

    public async Task<Equipment> AddAsync(Equipment equipment, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Equipment.AddAsync(equipment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<Equipment> UpdateAsync(Equipment equipment, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Equipment.FindAsync([equipment.Id], cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"Equipment with ID '{equipment.Id}' was not found.");
        }

        entity.Name = equipment.Name;
        entity.CategoryId = equipment.CategoryId;
        entity.Description = equipment.Description;
        entity.PricePerDay = equipment.PricePerDay;
        entity.UpdatedBy = equipment.UpdatedBy;
        entity.UpdatedAt = equipment.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Equipment.FindAsync([id], cancellationToken);
        if (existing is null) return false;

        _context.Equipment.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
