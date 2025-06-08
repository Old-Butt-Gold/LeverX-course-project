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

    public MongoReviewRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings)
    {
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
        UpdatedBy = embedded.UpdatedBy
    };
}
