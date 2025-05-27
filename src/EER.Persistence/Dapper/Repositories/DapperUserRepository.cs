using System.Data;
using System.Data.Common;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperUserRepository : IUserRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperUserRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Identity].[User]";
        return await _connection.QueryAsync<User>(new CommandDefinition(sql, transaction: _transaction,
            cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Identity].[User] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql,
            new { Id = id }, transaction: _transaction,
            cancellationToken: cancellationToken));
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               INSERT INTO [Identity].[User] (Email, PasswordHash, FullName, UserRole)
                               OUTPUT INSERTED.*
                               VALUES (@Email, @PasswordHash, @FullName, @UserRole)
                           """;

        var parameters = new
        {
            user.Email,
            user.PasswordHash,
            user.FullName,
            UserRole = user.UserRole.ToString()
        };

        return await _connection.QuerySingleAsync<User>(
            new CommandDefinition(sql, parameters, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = """
                                UPDATE [Identity].[User]
                                SET
                                    Email = @Email,
                                    PasswordHash = @PasswordHash,
                                    FullName = @FullName,
                                    UserRole = @UserRole,
                                    UpdatedAt = GETUTCDATE()
                                OUTPUT INSERTED.*
                                WHERE Id = @Id
                           """;

        var parameters = new
        {
            user.Id,
            user.Email,
            user.PasswordHash,
            user.FullName,
            UserRole = user.UserRole.ToString()
        };

        return await _connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(sql, parameters, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Identity].[User] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id },
                cancellationToken: cancellationToken, transaction: _transaction));
        return affectedRows > 0;
    }
}
