﻿using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
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

    public async Task<IEnumerable<Office>> GetAllAsync(ITransaction? transaction = null, CancellationToken ct = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Office?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var document = await _collection.Find(o => o.Id == id).FirstOrDefaultAsync(ct);
        return document != null ? MapToEntity(document) : null;
    }

    public async Task<Office> AddAsync(Office office, ITransaction? transaction = null, CancellationToken ct = default)
    {
        office.Id = await _idGenerator.GetNextIdAsync(_settings.OfficeCollection);

        var document = MapToDocument(office);

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

    public async Task<Office> UpdateAsync(Office office, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<OfficeDocument>.Filter.Eq(o => o.Id, office.Id);

        var update = Builders<OfficeDocument>.Update
            .Set(o => o.Address, office.Address)
            .Set(o => o.City, office.City)
            .Set(o => o.Country, office.Country)
            .Set(o => o.IsActive, office.IsActive)
            .Set(o => o.UpdatedBy, office.UpdatedBy)
            .Set(o => o.UpdatedAt, office.UpdatedAt);

        var options = new FindOneAndUpdateOptions<OfficeDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        OfficeDocument document;

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
        var filter = Builders<OfficeDocument>.Filter.Eq(o => o.Id, id);
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
