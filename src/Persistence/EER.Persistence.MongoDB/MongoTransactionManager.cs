using System.Data;
using EER.Domain.DatabaseAbstractions.Transaction;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace EER.Persistence.MongoDB;

public class MongoTransactionManager : ITransactionManager
{
    private readonly IMongoDatabase _database;
    private readonly bool _supportsTransactions;

    public MongoTransactionManager(IMongoDatabase database)
    {
        // TODO MongoDB for now doesn't support transactions
        _database = database;
        _supportsTransactions = database.Client.Cluster.Description.Type != ClusterType.Standalone;
    }

    public async Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        if (!_supportsTransactions)
            return new DummyTransaction();

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

    internal class DummyTransaction : ITransaction
    {
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task RollbackAsync(CancellationToken ct = default) => Task.CompletedTask;
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
