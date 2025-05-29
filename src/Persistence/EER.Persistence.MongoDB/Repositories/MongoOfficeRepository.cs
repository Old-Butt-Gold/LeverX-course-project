using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.Office;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoOfficeRepository : IOfficeRepository
{
    private readonly IMongoCollection<OfficeDocument> _collection;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoOfficeRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<OfficeDocument>(_settings.OfficeCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<Office>> GetAllAsync(CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Office?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var document = await _collection.Find(o => o.Id == id).FirstOrDefaultAsync(ct);
        return document != null ? MapToEntity(document) : null;
    }

    public async Task<Office> AddAsync(Office office, CancellationToken ct = default)
    {
        office.Id = await _idGenerator.GetNextIdAsync(_settings.OfficeCollection);
        office.UpdatedAt = DateTime.UtcNow;
        office.CreatedAt = DateTime.UtcNow;

        var document = MapToDocument(office);
        await _collection.InsertOneAsync(document, cancellationToken: ct);
        return MapToEntity(document);
    }

    public async Task<Office?> UpdateAsync(Office office, CancellationToken ct = default)
    {
        var filter = Builders<OfficeDocument>.Filter.Eq(o => o.Id, office.Id);

        var update = Builders<OfficeDocument>.Update
            .Set(o => o.Address, office.Address)
            .Set(o => o.City, office.City)
            .Set(o => o.Country, office.Country)
            .Set(o => o.IsActive, office.IsActive)
            .Set(o => o.UpdatedBy, office.UpdatedBy)
            .Set(o => o.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<OfficeDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(
            filter, update, options, ct);

        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(o => o.Id == id, ct);
        return result.DeletedCount > 0;
    }

    private static OfficeDocument MapToDocument(Office entity) => new()
    {
        Id = entity.Id,
        OwnerId = entity.OwnerId,
        Address = entity.Address,
        City = entity.City,
        Country = entity.Country,
        IsActive = entity.IsActive,
        EquipmentItemIds = [],
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedAt = entity.UpdatedAt,
        UpdatedBy = entity.UpdatedBy
    };

    private static Office MapToEntity(OfficeDocument doc) => new()
    {
        Id = doc.Id,
        OwnerId = doc.OwnerId,
        Address = doc.Address,
        City = doc.City,
        Country = doc.Country,
        IsActive = doc.IsActive,
        CreatedAt = doc.CreatedAt,
        CreatedBy = doc.CreatedBy,
        UpdatedAt = doc.UpdatedAt,
        UpdatedBy = doc.UpdatedBy
    };
}
