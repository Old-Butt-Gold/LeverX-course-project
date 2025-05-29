using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal class EquipmentItemService : IEquipmentItemService
{
    private readonly IEquipmentItemRepository _equipmentItemRepository;

    public EquipmentItemService(IEquipmentItemRepository equipmentItemRepository)
    {
        _equipmentItemRepository = equipmentItemRepository;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _equipmentItemRepository.GetAllAsync(cancellationToken);
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _equipmentItemRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<EquipmentItem> CreateAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        return await _equipmentItemRepository.AddAsync(item, cancellationToken);
    }

    public async Task<EquipmentItem> UpdateAsync(long id, EquipmentItem updatedItem, CancellationToken cancellationToken = default)
    {
        var existingItem = await _equipmentItemRepository.GetByIdAsync(id, cancellationToken);
        if (existingItem is null)
            throw new KeyNotFoundException("EquipmentItem with provided ID is not found");

        existingItem.EquipmentId = updatedItem.EquipmentId;
        existingItem.OfficeId = updatedItem.OfficeId;
        existingItem.SerialNumber = updatedItem.SerialNumber;
        existingItem.ItemStatus = updatedItem.ItemStatus;
        existingItem.MaintenanceDate = updatedItem.MaintenanceDate;
        existingItem.PurchaseDate = updatedItem.PurchaseDate;
        existingItem.UpdatedBy = updatedItem.UpdatedBy;

        return await _equipmentItemRepository.UpdateAsync(existingItem, cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _equipmentItemRepository.DeleteAsync(id, cancellationToken);
    }
}
