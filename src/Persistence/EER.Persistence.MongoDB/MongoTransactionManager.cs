using System.Data;
using EER.Domain.DatabaseAbstractions.Transaction;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB;

public class MongoTransactionManager : ITransactionManager
{
    private readonly IMongoDatabase _database;

    public MongoTransactionManager(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        var session = await _database.Client.StartSessionAsync(cancellationToken: ct);

        session.StartTransaction();
        return new MongoTransaction(session);
    }

    internal class MongoTransaction : ITransaction
    {
        public IClientSessionHandle Session { get; init; }

        public MongoTransaction(IClientSessionHandle session)
        {
            Session = session;
        }

        public Task CommitAsync(CancellationToken ct = default)
        {
            return Session.CommitTransactionAsync(ct);
        }

        public Task RollbackAsync(CancellationToken ct = default)
        {
            return Session.AbortTransactionAsync(ct);
        }

        public void Dispose()
        {
            Session.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Session.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
