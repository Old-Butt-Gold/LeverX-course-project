using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperRentalRepository : IRentalRepository
{
    private readonly IDbConnection _connection;

    public DapperRentalRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Rental]";
        return await _connection.QueryAsync<Rental>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Rental] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Rental>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default)
    {
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
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task<Rental> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               DECLARE @UpdatedTable TABLE (
                                   Id INT,
                                   CustomerId UNIQUEIDENTIFIER,
                                   OwnerId UNIQUEIDENTIFIER,
                                   StartDate DATETIME2(0),
                                   EndDate DATETIME2(0),
                                   TotalPrice DECIMAL(9,2),
                                   Status NVARCHAR(255),
                                   CreatedAt DATETIME2(2),
                                   CreatedBy UNIQUEIDENTIFIER,
                                   UpdatedAt DATETIME2(2),
                                   UpdatedBy UNIQUEIDENTIFIER
                               );

                               UPDATE [Supplies].[Rental]
                               SET
                                   Status = @Status,
                                   UpdatedAt = @UpdatedAt,
                                   UpdatedBy = @UpdatedBy
                               OUTPUT
                                   INSERTED.Id,
                                   INSERTED.CustomerId,
                                   INSERTED.OwnerId,
                                   INSERTED.StartDate,
                                   INSERTED.EndDate,
                                   INSERTED.TotalPrice,
                                   INSERTED.Status,
                                   INSERTED.CreatedAt,
                                   INSERTED.CreatedBy,
                                   INSERTED.UpdatedAt,
                                   INSERTED.UpdatedBy
                               INTO @UpdatedTable
                               WHERE Id = @Id;

                               SELECT * FROM @UpdatedTable;
                           """;

        return await _connection.QuerySingleAsync<Rental>(
            new CommandDefinition(sql, new
            {
                Id = id,
                UpdatedAt = DateTime.UtcNow,
                Status = status.ToString(),
                UpdatedBy = updatedBy,
            }, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[Rental] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
