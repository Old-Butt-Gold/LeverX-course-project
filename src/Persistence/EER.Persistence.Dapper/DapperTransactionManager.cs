using System.Data;
using System.Data.Common;
using EER.Domain.DatabaseAbstractions.Transaction;

namespace EER.Persistence.Dapper;

public class DapperTransactionManager : ITransactionManager
{
    private readonly DbConnection _connection;

    public DapperTransactionManager(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync(ct);

        return new DapperTransaction(await _connection.BeginTransactionAsync(isolationLevel, ct));
    }

    internal class DapperTransaction : ITransaction
    {
        public DbTransaction Transaction { get; init; }

        public DapperTransaction(DbTransaction transaction)
        {
            Transaction = transaction;
        }

        public Task CommitAsync(CancellationToken ct = default)
        {
            return Transaction.CommitAsync(ct);
        }

        public Task RollbackAsync(CancellationToken ct = default)
        {
            return Transaction.RollbackAsync(ct);
        }

        public void Dispose() => Transaction.Dispose();

        public async ValueTask DisposeAsync()
        {
            await Transaction.DisposeAsync();
        }
    }
}
