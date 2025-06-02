using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IEquipmentItemRepository : IRepository<EquipmentItem, long>
{
    Task<EquipmentItem> UpdateAsync(EquipmentItem item, CancellationToken cancellationToken = default);
}
