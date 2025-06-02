using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IRentalRepository : IRepository<Rental, int>
{
    Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
