using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoEquipmentRepository : IEquipmentRepository
{
    public Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Equipment> AddAsync(Equipment equipment, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Equipment?> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
