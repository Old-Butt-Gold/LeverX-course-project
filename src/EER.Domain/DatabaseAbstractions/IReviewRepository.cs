using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IReviewRepository
{
    Task<Review> AddAsync(Review review, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetReviewsByEquipmentIdAsync(int equipmentId, ITransaction? transaction = null, CancellationToken ct = default);
    Task<Review?> GetReviewAsync(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken ct = default);
    Task<bool> IsExistsReview(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken ct = default);
    Task<bool> DeleteReviewAsync(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
