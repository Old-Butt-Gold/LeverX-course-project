using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IRentalRepository : IRepository<Rental, int>
{
    Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default);
    Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
