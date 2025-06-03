using System.Data;

namespace EER.Domain.DatabaseAbstractions.Transaction;

public interface ITransactionManager
{
    Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);
}
