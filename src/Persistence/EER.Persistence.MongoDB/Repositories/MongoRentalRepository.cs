using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
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

    public async Task<IEnumerable<Rental>> GetAllAsync(ITransaction? transaction = null, CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Rental?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var document = await _collection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<Rental> AddAsync(Rental rental, ITransaction? transaction = null, CancellationToken ct = default)
    {
        rental.Id = await _idGenerator.GetNextIdAsync(_settings.RentalCollection);

        var document = MapToDocument(rental);

        var options = new InsertOneOptions();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _collection.InsertOneAsync(session, document, options, ct);
        }
        else
        {
            await _collection.InsertOneAsync(document, options, ct);
        }

        return MapToEntity(document);
    }

    public async Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, ITransaction? transaction = null, CancellationToken ct = default)
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

        RentalDocument document;

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            document = await _collection.FindOneAndUpdateAsync(session, filter, update, options, ct);
        }
        else
        {
            document = await _collection.FindOneAndUpdateAsync(filter, update, options, ct);
        }

        return MapToEntity(document);
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<RentalDocument>.Filter.Eq(r => r.Id, id);
        var options = new DeleteOptions();

        DeleteResult result;

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            result = await _collection.DeleteOneAsync(session, filter, options, ct);
        }
        else
        {
            result = await _collection.DeleteOneAsync(filter, ct);
        }

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
