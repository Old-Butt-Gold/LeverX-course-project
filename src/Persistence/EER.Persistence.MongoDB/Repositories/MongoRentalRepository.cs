using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;
using EER.Persistence.MongoDB.Documents.Rental;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoRentalRepository : IRentalRepository
{
    private readonly IMongoCollection<RentalDocument> _collection;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoRentalRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<RentalDocument>(_settings.RentalCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var document = await _collection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<Rental> AddAsync(Rental rental, CancellationToken ct = default)
    {
        rental.Id = await _idGenerator.GetNextIdAsync(_settings.RentalCollection);

        var document = MapToDocument(rental);
        await _collection.InsertOneAsync(document, cancellationToken: ct);
        return MapToEntity(document);
    }

    public async Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, CancellationToken ct = default)
    {
        var id = rentalToUpdate.Id;

        var filter = Builders<RentalDocument>.Filter.Eq(r => r.Id, id);

        var update = Builders<RentalDocument>.Update
            .Set(r => r.Status, rentalToUpdate.Status)
            .Set(r => r.UpdatedBy, rentalToUpdate.UpdatedBy)
            .Set(r => r.UpdatedAt, rentalToUpdate.UpdatedAt);

        // TODO Handle changing EquipmentItems ItemState in Available after RentalStatus: [Canceled, Completed]

        var options = new FindOneAndUpdateOptions<RentalDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(filter, update, options, ct);

        return MapToEntity(document);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(r => r.Id == id, ct);
        return result.DeletedCount > 0;
    }

    // TODO arrays
    private static RentalDocument MapToDocument(Rental entity) => new()
    {
        Id = entity.Id,
        CustomerId = entity.CustomerId,
        OwnerId = entity.OwnerId,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        TotalPrice = entity.TotalPrice,
        Status = entity.Status,
        Items = [],
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedAt = entity.UpdatedAt,
        UpdatedBy = entity.UpdatedBy
    };

    private static Rental MapToEntity(RentalDocument doc) => new()
    {
        Id = doc.Id,
        CustomerId = doc.CustomerId,
        OwnerId = doc.OwnerId,
        StartDate = doc.StartDate,
        EndDate = doc.EndDate,
        TotalPrice = doc.TotalPrice,
        Status = doc.Status,
        CreatedAt = doc.CreatedAt,
        CreatedBy = doc.CreatedBy,
        UpdatedAt = doc.UpdatedAt,
        UpdatedBy = doc.UpdatedBy
    };

}
