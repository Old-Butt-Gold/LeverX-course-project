using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.User;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;
    private readonly IMongoDatabase _database;

    public MongoUserRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings)
    {
        _database = database;
        _collection = database.GetCollection<UserDocument>(settings.Value.UserCollection);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find("{}").ToListAsync(cancellationToken);
        return documents.Select(MapToEntity);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is not null ? MapToEntity(document) : null;
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.Id = Guid.Empty;

        var document = MapToDocument(user);
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
        return MapToEntity(document);
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, user.Id);

        var update = Builders<UserDocument>.Update
            .Set(u => u.Email, user.Email)
            .Set(u => u.FullName, user.FullName)
            .Set(u => u.UserRole, user.UserRole)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<UserDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(
            filter, update, options, cancellationToken);

        return document != null ? MapToEntity(document) : null;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(u => u.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

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
