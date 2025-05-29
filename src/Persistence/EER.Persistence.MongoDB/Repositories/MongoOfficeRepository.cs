using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoOfficeRepository : IOfficeRepository
{
    public Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Office> AddAsync(Office office, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Office?> UpdateAsync(Office office, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
