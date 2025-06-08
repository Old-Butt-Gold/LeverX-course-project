using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.Equipment;
using EER.Persistence.MongoDB.Documents.EquipmentItem;
using EER.Persistence.MongoDB.Documents.Rental;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoEquipmentItemRepository : IEquipmentItemRepository
{
    private readonly IMongoCollection<EquipmentItemDocument> _collection;
    private readonly IMongoCollection<EquipmentDocument> _equipmentCollection;

    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoEquipmentItemRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<EquipmentItemDocument>(_settings.EquipmentItemCollection);
        _equipmentCollection = database.GetCollection<EquipmentDocument>(_settings.EquipmentCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(ITransaction? transaction = null, CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var document = await _collection.Find(i => i.Id == id).FirstOrDefaultAsync(ct);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<EquipmentItem> AddAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken ct = default)
    {
        item.Id = await _idGenerator.GetNextLongIdAsync(_settings.EquipmentItemCollection);

        var document = MapToDocument(item);

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

    public async Task<EquipmentItem> UpdateAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<EquipmentItemDocument>.Filter.Eq(i => i.Id, item.Id);

        var update = Builders<EquipmentItemDocument>.Update
            .Set(i => i.OfficeId, item.OfficeId)
            .Set(i => i.SerialNumber, item.SerialNumber)
            .Set(i => i.Status, item.ItemStatus)
            .Set(i => i.MaintenanceDate, item.MaintenanceDate.HasValue
                ? DateOnly.FromDateTime(item.MaintenanceDate.Value)
                : null)
            .Set(i => i.PurchaseDate, DateOnly.FromDateTime(item.PurchaseDate))
            .Set(i => i.UpdatedBy, item.UpdatedBy)
            .Set(i => i.UpdatedAt, item.UpdatedAt);

        var options = new FindOneAndUpdateOptions<EquipmentItemDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        EquipmentItemDocument document;

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

    public async Task<IEnumerable<EquipmentItem>> GetByIdsWithEquipmentAsync(IEnumerable<long> ids, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        var filter = Builders<EquipmentItemDocument>.Filter.In(i => i.Id, ids);

        var documents = session != null
            ? await _collection.Find(session, filter).ToListAsync(cancellationToken)
            : await _collection.Find(filter).ToListAsync(cancellationToken);

        var equipmentIds = documents
            .Select(d => d.EquipmentId)
            .Distinct();

        var equipmentFilter = Builders<EquipmentDocument>.Filter.In(e => e.Id, equipmentIds);

        var equipmentDocuments = session != null
            ? await _equipmentCollection.Find(session, equipmentFilter).ToListAsync(cancellationToken)
            : await _equipmentCollection.Find(equipmentFilter).ToListAsync(cancellationToken);

        var equipmentDict = equipmentDocuments.ToDictionary(e => e.Id);

        return documents.Select(doc =>
        {
            var item = MapToEntity(doc);

            if (equipmentDict.TryGetValue(doc.EquipmentId, out var equipmentDocument))
            {
                item.Equipment = MongoEquipmentRepository.MapToEntity(equipmentDocument);
            }

            return item;
        });
    }

    public async Task<bool> DeleteAsync(long id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<EquipmentItemDocument>.Filter.Eq(i => i.Id, id);
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

    private static EquipmentItemDocument MapToDocument(EquipmentItem entity) => new()
    {
        Id = entity.Id,
        EquipmentId = entity.EquipmentId,
        OfficeId = entity.OfficeId,
        SerialNumber = entity.SerialNumber,
        Status = entity.ItemStatus,
        MaintenanceDate = entity.MaintenanceDate.HasValue
            ? DateOnly.FromDateTime(entity.MaintenanceDate.Value)
            : null,
        PurchaseDate = DateOnly.FromDateTime(entity.PurchaseDate),
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedAt = entity.UpdatedAt,
        UpdatedBy = entity.UpdatedBy
    };

    private static EquipmentItem MapToEntity(EquipmentItemDocument doc) => new()
    {
        Id = doc.Id,
        EquipmentId = doc.EquipmentId,
        OfficeId = doc.OfficeId,
        SerialNumber = doc.SerialNumber,
        ItemStatus = doc.Status,
        MaintenanceDate = doc.MaintenanceDate?.ToDateTime(TimeOnly.MinValue),
        PurchaseDate = doc.PurchaseDate.ToDateTime(TimeOnly.MinValue),
        CreatedAt = doc.CreatedAt,
        CreatedBy = doc.CreatedBy,
        UpdatedAt = doc.UpdatedAt,
        UpdatedBy = doc.UpdatedBy
    };

}
