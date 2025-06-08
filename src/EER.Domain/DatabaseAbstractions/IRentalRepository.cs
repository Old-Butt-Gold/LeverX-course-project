using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Domain.DatabaseAbstractions;

public interface IRentalRepository : IRepository<Rental, int>
{
    Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId, Role userRole, ITransaction? transaction = null,
        CancellationToken cancellationToken = default);
    Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<Rental?> GetByIdWithItemsAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
