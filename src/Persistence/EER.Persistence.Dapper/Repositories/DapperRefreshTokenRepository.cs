using System.Data.Common;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DbConnection _connection;

    public DapperRefreshTokenRepository(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT *
            FROM [Identity].[RefreshToken]
            WHERE Token = @Token
        """;

        return await _connection.QuerySingleOrDefaultAsync<RefreshToken>(
            new CommandDefinition(
                sql,
                new { Token = token },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task AddAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO [Identity].[RefreshToken]
                (UserId, Token, CreatedAt, ExpiresAt, RevokedAt)
            VALUES
                (@UserId, @Token, @CreatedAt, @ExpiresAt, @RevokedAt)
        """;

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    refreshToken.UserId,
                    refreshToken.Token,
                    refreshToken.CreatedAt,
                    refreshToken.ExpiresAt,
                    refreshToken.RevokedAt
                },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task UpdateAsync(RefreshToken refreshToken, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE [Identity].[RefreshToken]
            SET
                Token = @Token
            WHERE
                Id = @Id
        """;

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    refreshToken.Id,
                    refreshToken.Token
                },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task RevokeAllForUserAsync(Guid userId, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE [Identity].[RefreshToken]
            SET
                RevokedAt = GETUTCDATE()
            WHERE
                UserId = @UserId AND
                RevokedAt IS NULL
        """;

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { UserId = userId },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task RevokeTokenAsync(string token, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE [Identity].[RefreshToken]
            SET
                RevokedAt = GETUTCDATE()
            WHERE
                Token = @Token AND
                RevokedAt IS NULL
        """;

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { Token = token },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }
}
