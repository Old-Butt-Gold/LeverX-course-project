using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.EquipmentItem;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoEquipmentItemRepository : IEquipmentItemRepository
{
    private readonly IMongoCollection<EquipmentItemDocument> _collection;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoEquipmentItemRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<EquipmentItemDocument>(_settings.EquipmentItemCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var document = await _collection.Find(i => i.Id == id).FirstOrDefaultAsync(ct);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<EquipmentItem> AddAsync(EquipmentItem item, CancellationToken ct = default)
    {
        item.Id = await _idGenerator.GetNextLongIdAsync(_settings.EquipmentItemCollection);

        var document = MapToDocument(item);
        await _collection.InsertOneAsync(document, cancellationToken: ct);
        return MapToEntity(document);
    }

    public async Task<EquipmentItem> UpdateAsync(EquipmentItem item, CancellationToken ct = default)
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
            .Set(i => i.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<EquipmentItemDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(filter, update, options, ct);

        return MapToEntity(document);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(i => i.Id == id, ct);
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
