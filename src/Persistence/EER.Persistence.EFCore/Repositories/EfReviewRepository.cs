using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

public class EfReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public EfReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Review> AddAsync(Review review, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<IEnumerable<Review>> GetReviewsByEquipmentIdAsync(int equipmentId, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.EquipmentId == equipmentId)
            .Include(r => r.Customer)
            .ToListAsync(ct);

        return reviews;
    }

    public async Task<bool> IsExistsReview(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken ct = default)
    {
        return await _context.Reviews
            .Where(x => x.EquipmentId == equipmentId && x.CustomerId == customerId)
            .AsNoTracking()
            .AnyAsync(cancellationToken: ct);
    }
}
