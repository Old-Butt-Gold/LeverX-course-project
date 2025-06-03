using System.Data;
using EER.Domain.DatabaseAbstractions.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EER.Persistence.EFCore;

public class EfTransactionManager : ITransactionManager
{
    private readonly ApplicationDbContext _context;

    public EfTransactionManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationlevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(isolationlevel, ct);
        return new EfTransaction(transaction);
    }

    internal class EfTransaction : ITransaction
    {
        public IDbContextTransaction Transaction { get; init; }

        public EfTransaction(IDbContextTransaction transaction)
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
