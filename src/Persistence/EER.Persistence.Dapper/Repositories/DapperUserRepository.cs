using System.Data.Common;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperUserRepository : IUserRepository
{
    private readonly DbConnection _connection;

    public DapperUserRepository(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<User>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Identity].[User]";
        return await _connection.QueryAsync<User>(new CommandDefinition(sql, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
            cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByIdAsync(Guid id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Identity].[User] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql,
            new { Id = id }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
            cancellationToken: cancellationToken));
    }

    public async Task<User> AddAsync(User user, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               INSERT INTO [Identity].[User] (Email, PasswordHash, UserRole)
                               OUTPUT INSERTED.*
                               VALUES (@Email, @PasswordHash, @UserRole)
                           """;

        var parameters = new
        {
            user.Email,
            user.PasswordHash,
            UserRole = user.UserRole.ToString()
        };

        return await _connection.QuerySingleAsync<User>(
            new CommandDefinition(sql, parameters, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<User> UpdateAsync(User user, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                                UPDATE [Identity].[User]
                                SET
                                    Email = @Email,
                                    FullName = @FullName,
                                    UpdatedAt = @UpdatedAt
                                OUTPUT INSERTED.*
                                WHERE Id = @Id
                           """;

        var parameters = new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.UpdatedAt,
        };

        return await _connection.QuerySingleAsync<User>(
            new CommandDefinition(sql, parameters, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> IsEmailExists(string email, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               SELECT COUNT(1)
                               FROM [Identity].[User]
                               WHERE Email = @Email
                           """;

        var count = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new { Email = email },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );

        return count > 0;
    }

    public async Task<User?> GetByEmailAsync(string email, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               SELECT *
                               FROM [Identity].[User]
                               WHERE Email = @Email
                           """;

        return await _connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(
                sql,
                new { Email = email },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<bool> DeleteAsync(Guid id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Identity].[User] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
