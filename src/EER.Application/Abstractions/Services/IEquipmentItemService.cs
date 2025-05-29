using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IEquipmentItemService
{
    Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<EquipmentItem> CreateAsync(EquipmentItem item, CancellationToken cancellationToken = default);
    Task<EquipmentItem?> UpdateAsync(long id, EquipmentItem updatedItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
