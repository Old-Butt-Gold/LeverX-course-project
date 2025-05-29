using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoRentalRepository : IRentalRepository
{
    public Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Rental?> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
