using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperRentalRepository : IRentalRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperRentalRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Rental]";
        return await _connection.QueryAsync<Rental>(
            new CommandDefinition(sql, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Rental] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Rental>(
            new CommandDefinition(sql, new { Id = id }, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        // TODO CreatedBy
        const string sql = """
                               INSERT INTO [Supplies].[Rental] (
                                   CustomerId, OwnerId, StartDate, EndDate,
                                   TotalPrice, Status, CreatedBy, UpdatedBy
                               )
                               OUTPUT INSERTED.*
                               VALUES (
                                   @CustomerId, @OwnerId, @StartDate, @EndDate,
                                   @TotalPrice, @Status, @CreatedBy, @UpdatedBy
                               )
                           """;

        var parameters = new
        {
            rental.CustomerId,
            rental.OwnerId,
            rental.StartDate,
            rental.EndDate,
            rental.TotalPrice,
            Status = rental.Status.ToString(),
            rental.CreatedBy,
            rental.UpdatedBy
        };

        return await _connection.QuerySingleAsync<Rental>(
            new CommandDefinition(sql, parameters, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<Rental?> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               UPDATE [Supplies].[Rental]
                               SET
                                   Status = @Status,
                                   UpdatedBy = @UpdatedBy,
                                   UpdatedAt = GETUTCDATE(),
                               OUTPUT INSERTED.*
                               WHERE Id = @Id
                           """;

        // TODO UpdatedBy

        return await _connection.QuerySingleOrDefaultAsync<Rental>(
            new CommandDefinition(sql, new
            {
                Id = id,
                Status = status.ToString(),
                UpdatedBy = updatedBy,
            }, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[Rental] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, transaction: _transaction, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
