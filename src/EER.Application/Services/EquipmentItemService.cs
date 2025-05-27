using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal class EquipmentItemService : IEquipmentItemService
{
    private readonly IUnitOfWork _uow;

    public EquipmentItemService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentItemRepository.GetAllAsync(cancellationToken);
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentItemRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<EquipmentItem> CreateAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentItemRepository.AddAsync(item, cancellationToken);
    }

    public async Task<EquipmentItem?> UpdateAsync(long id, EquipmentItem updatedItem, CancellationToken cancellationToken = default)
    {
        var existingItem = await _uow.EquipmentItemRepository.GetByIdAsync(id, cancellationToken);
        if (existingItem == null) return null;

        existingItem.EquipmentId = updatedItem.EquipmentId;
        existingItem.OfficeId = updatedItem.OfficeId;
        existingItem.SerialNumber = updatedItem.SerialNumber;
        existingItem.ItemStatus = updatedItem.ItemStatus;
        existingItem.MaintenanceDate = updatedItem.MaintenanceDate;
        existingItem.PurchaseDate = updatedItem.PurchaseDate;
        existingItem.UpdatedBy = updatedItem.UpdatedBy;

        return await _uow.EquipmentItemRepository.UpdateAsync(existingItem, cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentItemRepository.DeleteAsync(id, cancellationToken);
    }
}
