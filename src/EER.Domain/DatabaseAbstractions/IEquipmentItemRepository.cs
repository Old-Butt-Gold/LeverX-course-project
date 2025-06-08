using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IEquipmentItemRepository : IRepository<EquipmentItem, long>
{
    Task<EquipmentItem> UpdateAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentItem>> GetByIdsWithEquipmentAsync(IEnumerable<long> ids, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
