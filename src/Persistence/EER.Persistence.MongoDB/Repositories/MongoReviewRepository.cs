using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.Equipment;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoReviewRepository : IReviewRepository
{
    private readonly IMongoCollection<EquipmentDocument> _equipmentCollection;
    private readonly DatabaseSettings _settings;
    private readonly IUserRepository _userRepository;

    public MongoReviewRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings,
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _settings = settings.Value;
        _equipmentCollection = database.GetCollection<EquipmentDocument>(_settings.EquipmentCollection);
    }

    public async Task<Review> AddAsync(Review review, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var reviewEmbedded = MapToEmbedded(review);
        var equipmentId = review.EquipmentId;

        var equipment = await _equipmentCollection
            .Find(e => e.Id == equipmentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (equipment is null)
        {
            throw new KeyNotFoundException($"Equipment with this ID: {equipmentId} wasn't found");
        }

        var filter = Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipmentId);

        var allRatings = equipment.Reviews
            .Select(x => (decimal)x.Rating)
            .Append(review.Rating);

        var averageRating = allRatings.Any() ? allRatings.Average() : review.Rating;

        var update = Builders<EquipmentDocument>.Update
            .Push(e => e.Reviews, reviewEmbedded)
            .Inc(e => e.TotalReviews, 1)
            .Set(e => e.AverageRating, averageRating);

        var options = new UpdateOptions();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _equipmentCollection.UpdateOneAsync(
                session, filter, update, options, cancellationToken);
        }
        else
        {
            await _equipmentCollection.UpdateOneAsync(
                filter, update, options, cancellationToken);
        }

        return review;
    }

    public async Task<IEnumerable<Review>> GetReviewsByEquipmentIdAsync(int equipmentId, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipmentId);
        var equipment = await _equipmentCollection.Find(filter).FirstOrDefaultAsync(ct);

        if (equipment is null)
        {
            throw new KeyNotFoundException($"Equipment with ID {equipmentId} not found");
        }

        var customerIds = equipment.Reviews
            .Select(r => r.CustomerId)
            .Distinct()
            .ToList();

        if (customerIds.Count == 0)
            return [];

        var users = await _userRepository.GetByIdsAsync(customerIds, transaction, ct);
        var userById = users.ToDictionary(u => u.Id);

        return equipment.Reviews
            .Select(embedded =>
            {
                if (!userById.TryGetValue(embedded.CustomerId, out var user))
                    throw new KeyNotFoundException($"User with ID {embedded.CustomerId} not found");

                return MapFromEmbeddedWithUser(embedded, equipmentId, user);
            });
    }

    public async Task<Review?> GetReviewAsync(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<EquipmentDocument>.Filter.And(
            Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipmentId),
            Builders<EquipmentDocument>.Filter.ElemMatch(e => e.Reviews, r => r.CustomerId == customerId)
        );

        var equipment = await _equipmentCollection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken: ct);

        var embedded = equipment?.Reviews
            .First(r => r.CustomerId == customerId);

        return embedded != null
            ? MapFromEmbedded(embedded, equipmentId)
            : null;
    }

    public async Task<bool> IsExistsReview(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        var filter = Builders<EquipmentDocument>.Filter.And(
            Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipmentId),
            Builders<EquipmentDocument>.Filter.ElemMatch(e => e.Reviews,
                review => review.CustomerId == customerId)
        );

        var options = new FindOptions();

        var result = session != null
            ? await _equipmentCollection.Find(session, filter, options).FirstOrDefaultAsync(ct)
            : await _equipmentCollection.Find(filter, options).FirstOrDefaultAsync(ct);

        return result != null;
    }

    public async Task<bool> DeleteReviewAsync(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filterDoc =
            Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipmentId);

        var equipment = await _equipmentCollection
            .Find(filterDoc)
            .FirstOrDefaultAsync(cancellationToken);

        if (equipment is null)
            return false;

        var remainingRatings = equipment.Reviews
            .Where(r => r.CustomerId != customerId)
            .Select(r => (decimal)r.Rating)
            .ToList();

        var newTotal = remainingRatings.Count;
        var newAverage = newTotal > 0 ? remainingRatings.Average() : 0m;

        var update = Builders<EquipmentDocument>.Update
            .PullFilter(e => e.Reviews, r => r.CustomerId == customerId)
            .Set(e => e.TotalReviews, newTotal)
            .Set(e => e.AverageRating, newAverage);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        var result = session != null
            ? await _equipmentCollection.UpdateOneAsync(session, filterDoc, update, cancellationToken: cancellationToken)
            : await _equipmentCollection.UpdateOneAsync(filterDoc, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    private ReviewEmbedded MapToEmbedded(Review review) => new()
    {
        CustomerId = review.CustomerId,
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt,
        CreatedBy = review.CreatedBy,
        UpdatedAt = review.UpdatedAt,
        UpdatedBy = review.UpdatedBy
    };

    private Review MapFromEmbedded(ReviewEmbedded embedded, int equipmentId) => new()
    {
        EquipmentId = equipmentId,
        CustomerId = embedded.CustomerId,
        Rating = embedded.Rating,
        Comment = embedded.Comment,
        CreatedAt = embedded.CreatedAt,
        CreatedBy = embedded.CreatedBy,
        UpdatedAt = embedded.UpdatedAt,
        UpdatedBy = embedded.UpdatedBy,
    };

    private Review MapFromEmbeddedWithUser(ReviewEmbedded embedded, int equipmentId, User user)
    {
        var review = MapFromEmbedded(embedded, equipmentId);
        review.Customer = user;
        return review;
    }
}
