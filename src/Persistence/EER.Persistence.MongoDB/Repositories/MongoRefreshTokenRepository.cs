using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Persistence.MongoDB.Documents.User;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IMongoCollection<UserDocument> _collection;
    private readonly DatabaseSettings _settings;
    private readonly IdGenerator _idGenerator;

    public MongoRefreshTokenRepository(IMongoDatabase database, IOptions<DatabaseSettings> settings, IdGenerator generator)
    {
        _idGenerator = generator;
        _settings = settings.Value;
        _collection = database.GetCollection<UserDocument>(_settings.UserCollection);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.ElemMatch(
            u => u.RefreshTokens,
            rt => rt.Token == token && rt.RevokedAt == null);

        var projection = Builders<UserDocument>.Projection
            .Include(u => u.RefreshTokens)
            .ElemMatch(u => u.RefreshTokens, rt => rt.Token == token);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        UserDocument? userDoc;
        if (session != null)
        {
            userDoc = await _collection.Find(session, filter)
                .Project<UserDocument>(projection)
                .FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            userDoc = await _collection.Find(filter)
                .Project<UserDocument>(projection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var tokenDoc = userDoc?.RefreshTokens.FirstOrDefault();

        if (tokenDoc is null)
            return null;

        var tokenDomain = MapToRefreshTokenEntity(tokenDoc);
        tokenDomain.UserId = userDoc!.Id;

        return tokenDomain;
    }

    public async Task AddAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var mappedToken = MapToRefreshTokenEmbedded(refreshToken);
        mappedToken.Id = await _idGenerator.GetNextLongIdAsync(_settings.RefreshTokensEmbedded);

        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, refreshToken.UserId);
        var update = Builders<UserDocument>.Update.Push(
            u => u.RefreshTokens, mappedToken);

        var options = new UpdateOptions { IsUpsert = false };

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _collection.UpdateOneAsync(session, filter, update, options, cancellationToken);
        }
        else
        {
            await _collection.UpdateOneAsync(filter, update, options, cancellationToken);
        }
    }

    public async Task UpdateAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(u => u.Id, refreshToken.UserId),
            Builders<UserDocument>.Filter.ElemMatch(
                u => u.RefreshTokens,
                rt => rt.Id == refreshToken.Id)
        );

        var update = Builders<UserDocument>.Update
            .Set("RefreshTokens.$.Token", refreshToken.Token);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _collection.UpdateOneAsync(session, filter, update, cancellationToken: cancellationToken);
        }
        else
        {
            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }

    public async Task RevokeAllForUserAsync(Guid userId, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.Set("RefreshTokens.$[].RevokedAt", DateTime.UtcNow);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _collection.UpdateOneAsync(session, filter, update, cancellationToken: cancellationToken);
        }
        else
        {
            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }

    public async Task RevokeTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserDocument>.Filter.ElemMatch(
            u => u.RefreshTokens,
            rt => rt.Token == token && rt.RevokedAt == null);

        var update = Builders<UserDocument>.Update
            .Set("RefreshTokens.$.RevokedAt", DateTime.UtcNow);

        var session = (transaction as MongoTransactionManager.MongoTransaction)?.Session;

        if (session != null)
        {
            await _collection.UpdateOneAsync(session, filter, update, cancellationToken: cancellationToken);
        }
        else
        {
            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }

    private static RefreshTokenEmbedded MapToRefreshTokenEmbedded(RefreshToken entity) => new()
    {
        Token = entity.Token,
        CreatedAt = entity.CreatedAt,
        ExpiresAt = entity.ExpiresAt,
        RevokedAt = entity.RevokedAt
    };

    private static RefreshToken MapToRefreshTokenEntity(RefreshTokenEmbedded doc) => new()
    {
        Id = doc.Id,
        Token = doc.Token,
        CreatedAt = doc.CreatedAt,
        ExpiresAt = doc.ExpiresAt,
        RevokedAt = doc.RevokedAt,
    };
}
