using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoEquipmentItemRepository : IEquipmentItemRepository
{
    public Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<EquipmentItem> AddAsync(EquipmentItem item, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<EquipmentItem?> UpdateAsync(EquipmentItem item, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
