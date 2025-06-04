using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.User;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

internal sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;
    private readonly IMongoDatabase _database;

    public MongoUserRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings)
    {
        _database = database;
        _collection = database.GetCollection<UserDocument>(settings.Value.UserCollection);
    }

    public async Task<IEnumerable<User>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        // TODO transactions with reading?
        var documents = await _collection.Find("{}").ToListAsync(cancellationToken);
        return documents.Select(MapToEntity);
    }

    public async Task<User?> GetByIdAsync(Guid id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        // TODO transactions with reading?
        var document = await _collection.Find(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<User> AddAsync(User user, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var document = MapToDocument(user);
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

    public async Task<User> UpdateAsync(User user, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, user.Id);

        var update = Builders<UserDocument>.Update
            .Set(u => u.Email, user.Email)
            .Set(u => u.FullName, user.FullName)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<UserDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        UserDocument userDocument;

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            userDocument = await _collection.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken);
        }
        else
        {
            userDocument = await _collection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
        }

        return MapToEntity(userDocument);
    }

    public async Task<bool> IsEmailExistsAsync(string email, Guid? excludeUserId = null, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Email, email);

        if (excludeUserId.HasValue)
        {
            filter &= Builders<UserDocument>.Filter.Ne(c => c.Id, excludeUserId.Value);
        }

        var options = new CountOptions();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            return await _collection.CountDocumentsAsync(
                session, filter, options, cancellationToken) > 0;
        }

        return await _collection.CountDocumentsAsync(filter, options, cancellationToken) > 0;
    }

    public async Task<User?> GetByEmailAsync(string email, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.Regex(
            u => u.Email, email);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        UserDocument? document;

        if (session != null)
        {
            document = await _collection
                .Find(session, filter)
                .FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            document = await _collection
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return document is not null
            ? MapToEntity(document)
            : null;
    }

    public async Task<bool> DeleteAsync(Guid id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        DeleteResult result;
        DeleteOptions deleteOptions = new();

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            result = await _collection.DeleteOneAsync(
                session, u => u.Id == id, deleteOptions, cancellationToken);
        }
        else
        {
            result = await _collection.DeleteOneAsync(u => u.Id == id, cancellationToken);
        }

        return result.DeletedCount > 0;
    }

    // TODO arrays
    private static UserDocument MapToDocument(User entity) => new()
    {
        Id = entity.Id,
        Email = entity.Email,
        PasswordHash = entity.PasswordHash,
        FullName = entity.FullName,
        UserRole = entity.UserRole,
        Favorites = [],
        OfficeIds = [],
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static User MapToEntity(UserDocument doc) => new()
    {
        Id = doc.Id,
        Email = doc.Email,
        PasswordHash = doc.PasswordHash,
        FullName = doc.FullName,
        UserRole = doc.UserRole,
        CreatedAt = doc.CreatedAt,
        UpdatedAt = doc.UpdatedAt
    };
}
