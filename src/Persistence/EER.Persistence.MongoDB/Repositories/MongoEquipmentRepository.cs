using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.Equipment;
using EER.Persistence.MongoDB.Documents.EquipmentItem;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoEquipmentRepository : IEquipmentRepository
{
    private readonly IMongoCollection<EquipmentDocument> _collection;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoEquipmentRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<EquipmentDocument>(_settings.EquipmentCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var document = await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(ct);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        var documents = await _collection
            .Find(e => e.CategoryId == categoryId)
            .ToListAsync(ct);

        return documents.Select(MapToEntity);
    }

    public async Task<Equipment> AddAsync(Equipment equipment, CancellationToken ct = default)
    {
        equipment.Id = await _idGenerator.GetNextIdAsync(_settings.EquipmentCollection);
        equipment.UpdatedAt = DateTime.UtcNow;
        equipment.CreatedAt = DateTime.UtcNow;

        var document = MapToDocument(equipment);
        await _collection.InsertOneAsync(document, cancellationToken: ct);
        return MapToEntity(document);
    }

    public async Task<Equipment> UpdateAsync(Equipment equipment, CancellationToken ct = default)
    {
        var filter = Builders<EquipmentDocument>.Filter.Eq(e => e.Id, equipment.Id);

        var update = Builders<EquipmentDocument>.Update
            .Set(e => e.Name, equipment.Name)
            .Set(e => e.CategoryId, equipment.CategoryId)
            .Set(e => e.Description, equipment.Description)
            .Set(e => e.PricePerDay, equipment.PricePerDay)
            .Set(e => e.UpdatedBy, equipment.UpdatedBy)
            .Set(e => e.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<EquipmentDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(
            filter, update, options, ct);

        return MapToEntity(document);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(e => e.Id == id, ct);
        return result.DeletedCount > 0;
    }

    // TODO arrays
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

    private static Equipment MapToEntity(EquipmentDocument doc) => new()
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
}
