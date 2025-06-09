using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.Category;
using EER.Persistence.MongoDB.Documents.Equipment;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoEquipmentRepository : IEquipmentRepository
{
    private readonly IMongoCollection<EquipmentDocument> _collection;
    private readonly IMongoCollection<CategoryDocument> _categoryCollection;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoEquipmentRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<EquipmentDocument>(_settings.EquipmentCollection);
        _categoryCollection = database.GetCollection<CategoryDocument>(_settings.CategoryCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(ITransaction? transaction = null, CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Equipment?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var document = await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(ct);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<Equipment> AddAsync(Equipment equipment, ITransaction? transaction = null, CancellationToken ct = default)
    {
        equipment.Id = await _idGenerator.GetNextIdAsync(_settings.EquipmentCollection);

        var document = MapToDocument(equipment);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;
        var options = new InsertOneOptions();

        if (session != null)
        {
            await _collection.InsertOneAsync(session, document, options, ct);
        }
        else
        {
            await _collection.InsertOneAsync(document, options, ct);
        }

        await UpdateCategoryEquipmentCount(
            equipment.CategoryId, increment: 1, session, cancellationToken: ct);

        return MapToEntity(document);
    }

    public async Task<Equipment> UpdateAsync(Equipment equipment, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var oldEquipment = await GetByIdAsync(equipment.Id, transaction, ct);

        var filter = Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipment.Id);

        var update = Builders<EquipmentDocument>.Update
            .Set(e => e.Name, equipment.Name)
            .Set(e => e.CategoryId, equipment.CategoryId)
            .Set(e => e.Description, equipment.Description)
            .Set(e => e.PricePerDay, equipment.PricePerDay)
            .Set(e => e.IsModerated, equipment.IsModerated)
            .Set(e => e.UpdatedBy, equipment.UpdatedBy)
            .Set(e => e.UpdatedAt, equipment.UpdatedAt);

        var options = new FindOneAndUpdateOptions<EquipmentDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;
        EquipmentDocument document;

        if (session != null)
        {
            document = await _collection.FindOneAndUpdateAsync(
                session, filter, update, options, ct);
        }
        else
        {
            document = await _collection.FindOneAndUpdateAsync(filter, update, options, ct);
        }

        if (oldEquipment!.CategoryId != equipment.CategoryId)
        {
            await UpdateCategoryEquipmentCount(oldEquipment.CategoryId, -1, session, ct);
            await UpdateCategoryEquipmentCount(equipment.CategoryId, +1, session, ct);
        }

        return MapToEntity(document);
    }

    public async Task<IEnumerable<Equipment>> GetUnmoderatedAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var documents = await _collection
            .Find(e => e.IsModerated == false)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToEntity);
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var equipment = await GetByIdAsync(id, transaction, ct);
        if (equipment is null) return false;

        var filter = Builders<EquipmentDocument>.Filter.Eq(e => e.Id, id);
        var options = new DeleteOptions();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;
        DeleteResult result;

        if (session != null)
        {
            result = await _collection.DeleteOneAsync(session, filter, options, cancellationToken: ct);
        }
        else
        {
            result = await _collection.DeleteOneAsync(filter, options, ct);
        }

        if (result.DeletedCount > 0)
        {
            await UpdateCategoryEquipmentCount(equipment.CategoryId, increment: -1, session, cancellationToken: ct);
        }

        return result.DeletedCount > 0;
    }

    private static EquipmentDocument MapToDocument(Equipment entity) => new()
    {
        Id = entity.Id,
        CategoryId = entity.CategoryId,
        OwnerId = entity.OwnerId,
        Name = entity.Name,
        Description = entity.Description,
        PricePerDay = entity.PricePerDay,
        AverageRating = entity.AverageRating,
        TotalReviews = entity.TotalReviews,
        IsModerated = entity.IsModerated,
        Images = [],
        Reviews = [],
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedAt = entity.UpdatedAt,
        UpdatedBy = entity.UpdatedBy
    };

    public static Equipment MapToEntity(EquipmentDocument doc) => new()
    {
        Id = doc.Id,
        CategoryId = doc.CategoryId,
        OwnerId = doc.OwnerId,
        Name = doc.Name,
        Description = doc.Description,
        PricePerDay = doc.PricePerDay,
        AverageRating = doc.AverageRating,
        TotalReviews = doc.TotalReviews,
        IsModerated = doc.IsModerated,
        CreatedAt = doc.CreatedAt,
        CreatedBy = doc.CreatedBy,
        UpdatedAt = doc.UpdatedAt,
        UpdatedBy = doc.UpdatedBy
    };

    private async Task UpdateCategoryEquipmentCount(int categoryId, int increment, IClientSessionHandle? session = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CategoryDocument>.Filter.Eq(c => c.Id, categoryId);
        var update = Builders<CategoryDocument>.Update.Inc(c => c.TotalEquipment, increment);

        var options = new UpdateOptions { IsUpsert = false };

        if (session != null)
        {
            await _categoryCollection.UpdateOneAsync(
                session, filter, update, options, cancellationToken);
        }
        else
        {
            await _categoryCollection.UpdateOneAsync(
                filter, update, options, cancellationToken);
        }
    }
}
