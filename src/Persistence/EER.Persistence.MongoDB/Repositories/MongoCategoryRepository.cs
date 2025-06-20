﻿using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
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

    public async Task<IEnumerable<Category>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(cancellationToken);
        return documents.Select(MapToEntity);
    }

    public async Task<Category?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<Category> AddAsync(Category category, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        category.Id = await _idGenerator.GetNextIdAsync(_settings.CategoryCollection);

        var document = MapToDocument(category);

        var options = new InsertOneOptions();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _collection.InsertOneAsync(session, document, options, cancellationToken);
        }
        else
        {
            await _collection.InsertOneAsync(document, options, cancellationToken);
        }

        return MapToEntity(document);
    }

    public async Task<Category> UpdateAsync(Category category, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CategoryDocument>.Filter.Eq(c => c.Id, category.Id);

        var update = Builders<CategoryDocument>.Update
            .Set(c => c.Name, category.Name)
            .Set(c => c.Description, category.Description)
            .Set(c => c.Slug, category.Slug)
            .Set(c => c.UpdatedBy, category.UpdatedBy)
            .Set(c => c.UpdatedAt, category.UpdatedAt);

        var options = new FindOneAndUpdateOptions<CategoryDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        CategoryDocument document;

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            document = await _collection.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken);
        }
        else
        {
            document = await _collection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
        }

        return MapToEntity(document);
    }

    public async Task<bool> IsSlugExistsAsync(string slug, int? excludeCategoryId = null, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CategoryDocument>.Filter.Eq(
            x => x.Slug, slug);

        if (excludeCategoryId.HasValue)
        {
            filter &= Builders<CategoryDocument>.Filter.Ne(c => c.Id, excludeCategoryId.Value);
        }

        var options = new CountOptions();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            return await _collection.CountDocumentsAsync(session, filter, options, cancellationToken) > 0;
        }

        return await _collection.CountDocumentsAsync(filter, options, cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CategoryDocument>.Filter.Eq(c => c.Id, id);

        var options = new DeleteOptions();

        DeleteResult result;

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            result = await _collection.DeleteOneAsync(session, filter, options, cancellationToken);
        }
        else
        {
            result = await _collection.DeleteOneAsync(filter, cancellationToken);
        }

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
