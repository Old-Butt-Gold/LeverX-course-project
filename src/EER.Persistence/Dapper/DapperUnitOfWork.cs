using System.Data.Common;
using EER.Domain.DatabaseAbstractions;
using EER.Persistence.Dapper.Repositories;

namespace EER.Persistence.Dapper;

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly DbConnection _connection;
    private DbTransaction? _transaction;

    private readonly Lazy<IUserRepository> _users;
    private readonly Lazy<ICategoryRepository> _categories;

    public DapperUnitOfWork(DbConnection connection)
    {
        _connection = connection;

        _users = new Lazy<IUserRepository>(() =>
            new DapperUserRepository(_connection, _transaction));

        _categories = new Lazy<ICategoryRepository>(() =>
            new DapperCategoryRepository(_connection, _transaction));
    }

    public IUserRepository UserRepository => _users.Value;
    public ICategoryRepository CategoryRepository => _categories.Value;

    public async Task BeginTransactionAsync()
    {
        _transaction = await _connection.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction not started");

        await _transaction.CommitAsync();
        await DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction is null) return;

        await _transaction.RollbackAsync();
        await DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _transaction?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_transaction is not null)
            await _transaction.DisposeAsync();
    }
}
