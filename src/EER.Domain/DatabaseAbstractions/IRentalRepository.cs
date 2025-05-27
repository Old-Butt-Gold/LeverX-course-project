using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Domain.DatabaseAbstractions;

public interface IRentalRepository : IRepository<Rental, int>
{
    Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default);
    Task<Rental?> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
