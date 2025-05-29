using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IEquipmentItemRepository : IRepository<EquipmentItem, long>
{
    Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<EquipmentItem> AddAsync(EquipmentItem item, CancellationToken cancellationToken = default);
    Task<EquipmentItem?> UpdateAsync(EquipmentItem item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
