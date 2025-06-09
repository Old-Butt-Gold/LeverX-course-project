using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Domain.Enums;
using EER.Persistence.MongoDB.Documents.EquipmentItem;
using EER.Persistence.MongoDB.Documents.Rental;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoRentalRepository : IRentalRepository
{
    private readonly IMongoCollection<RentalDocument> _rentalCollection;
    private readonly IEquipmentItemRepository _equipmentItemRepository;
    private readonly IdGenerator _idGenerator;
    private readonly DatabaseSettings _settings;

    public MongoRentalRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator idGenerator, IEquipmentItemRepository repository)
    {
        _settings = settings.Value;
        _rentalCollection = database.GetCollection<RentalDocument>(_settings.RentalCollection);
        _equipmentItemRepository = repository;
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(ITransaction? transaction = null, CancellationToken ct = default)
    {
        var documents = await _rentalCollection.Find("{}").ToListAsync(ct);
        return documents.Select(MapToEntity);
    }

    public async Task<Rental?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var document = await _rentalCollection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
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
            await _rentalCollection.InsertOneAsync(session, document, options, ct);
        }
        else
        {
            await _rentalCollection.InsertOneAsync(document, options, ct);
        }

        return MapToEntity(document);
    }

    public async Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId, Role userRole, ITransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        FilterDefinition<RentalDocument> filter = userRole switch
        {
            Role.Customer => Builders<RentalDocument>.Filter.Eq(d => d.CustomerId, userId),
            Role.Owner => Builders<RentalDocument>.Filter.Eq(d => d.OwnerId, userId),
            Role.Admin => Builders<RentalDocument>.Filter.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(userRole), $"Unsupported role: {userRole}")
        };

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        List<RentalDocument> docs =
            session != null
            ? await _rentalCollection
                .Find(session, filter)
                .ToListAsync(cancellationToken)
            : await _rentalCollection
                .Find(filter)
                .ToListAsync(cancellationToken);

        return docs.Select(MapToEntity);
    }

    public async Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, Guid manipulator, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var id = rentalToUpdate.Id;

        var filter = Builders<RentalDocument>.Filter.Eq(r => r.Id, id);

        var update = Builders<RentalDocument>.Update
            .Set(r => r.Status, rentalToUpdate.Status)
            .Set(r => r.UpdatedBy, rentalToUpdate.UpdatedBy)
            .Set(r => r.UpdatedAt, rentalToUpdate.UpdatedAt);

        var options = new FindOneAndUpdateOptions<RentalDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        RentalDocument updatedRentalDoc = session != null
            ? await _rentalCollection.FindOneAndUpdateAsync(session, filter, update, options, ct)
            : await _rentalCollection.FindOneAndUpdateAsync(filter, update, options, ct);

        if (rentalToUpdate.Status is RentalStatus.Canceled or RentalStatus.Completed)
        {
            var itemIds = updatedRentalDoc.Items.Select(i => i.EquipmentItemId);
            await _equipmentItemRepository.UpdateStatusForItemsAsync(itemIds, ItemStatus.Available, manipulator, transaction, ct);
        }

        return MapToEntity(updatedRentalDoc);
    }

    public async Task<Rental?> GetByIdWithItemsAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        var filter = Builders<RentalDocument>.Filter.Eq(r => r.Id, id);

        var document = session != null
            ? await _rentalCollection.Find(session, filter).FirstOrDefaultAsync(cancellationToken)
            : await _rentalCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return document is not null ? MapToEntityWithRentalItems(document) : null;
    }

    public async Task AddRentalItemsAsync(IEnumerable<RentalItem> items, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var rentalIds = items.Select(i => i.RentalId).Distinct().ToList();
        if (rentalIds.Count != 1)
        {
            throw new InvalidOperationException("All items must belong to the same rental");
        }

        var rentalId = rentalIds.Single();
        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        var filter = Builders<RentalDocument>.Filter.Eq(r => r.Id, rentalId);

        var update = Builders<RentalDocument>.Update
            .Set(r => r.Items, items.Select(item => new RentalItemEmbedded
            {
                EquipmentItemId = item.EquipmentItemId,
                ActualPrice = item.ActualPrice,
                CreatedAt = item.CreatedAt,
                CreatedBy = item.CreatedBy
            }).ToList());

        if (session != null)
        {
            await _rentalCollection.UpdateOneAsync(session, filter, update, cancellationToken: cancellationToken);
        }
        else
        {
            await _rentalCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken ct = default)
    {
        var filter = Builders<RentalDocument>.Filter.Eq(r => r.Id, id);
        var options = new DeleteOptions();

        DeleteResult result;

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            result = await _rentalCollection.DeleteOneAsync(session, filter, options, ct);
        }
        else
        {
            result = await _rentalCollection.DeleteOneAsync(filter, ct);
        }

        return result.DeletedCount > 0;
    }

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

    private static Rental MapToEntityWithRentalItems(RentalDocument doc)
    {
        var rental = MapToEntity(doc);

        rental.RentalItems = doc.Items.Select(i => new RentalItem
        {
            RentalId = doc.Id,
            EquipmentItemId = i.EquipmentItemId,
            ActualPrice = i.ActualPrice,
            CreatedAt = i.CreatedAt,
            CreatedBy = i.CreatedBy
        }).ToList();

        return rental;
    }
}
