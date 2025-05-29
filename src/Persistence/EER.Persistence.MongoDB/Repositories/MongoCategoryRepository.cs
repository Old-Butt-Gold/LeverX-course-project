using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.Category;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoCategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<CategoryDocument> _collection;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoCategoryRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator)
    {
        _settings = settings.Value;
        _collection = database.GetCollection<CategoryDocument>(settings.Value.CategoryCollection);
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(cancellationToken);
        return documents.Select(MapToEntity);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        category.Id = await _idGenerator.GetNextIdAsync(_settings.CategoryCollection);

        var document = MapToDocument(category);
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
        return MapToEntity(document);
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CategoryDocument>.Filter.Eq(c => c.Id, category.Id);

        var update = Builders<CategoryDocument>.Update
            .Set(c => c.Name, category.Name)
            .Set(c => c.Description, category.Description)
            .Set(c => c.Slug, category.Slug)
            .Set(c => c.UpdatedBy, category.UpdatedBy)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<CategoryDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(
            filter, update, options, cancellationToken);

        return MapToEntity(document);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(c => c.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    private static CategoryDocument MapToDocument(Category entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Slug = entity.Slug,
        TotalEquipment = entity.TotalEquipment,
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedAt = entity.UpdatedAt,
        UpdatedBy = entity.UpdatedBy,

    };

    private static Category MapToEntity(CategoryDocument doc) => new()
    {
        Id = doc.Id,
        Name = doc.Name,
        Description = doc.Description,
        Slug = doc.Slug,
        TotalEquipment = doc.TotalEquipment,
        CreatedAt = doc.CreatedAt,
        CreatedBy = doc.CreatedBy,
        UpdatedAt = doc.UpdatedAt,
        UpdatedBy = doc.UpdatedBy
    };
}
