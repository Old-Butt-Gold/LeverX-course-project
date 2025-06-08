using System.Data.Common;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperRentalRepository : IRentalRepository
{
    private readonly DbConnection _connection;

    public DapperRentalRepository(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Rental]";
        return await _connection.QueryAsync<Rental>(
            new CommandDefinition(sql, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<Rental?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Rental] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Rental>(
            new CommandDefinition(sql, new { Id = id }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<Rental> AddAsync(Rental rental, ITransaction? transaction = null, CancellationToken cancellationToken = default)
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
            new CommandDefinition(sql, parameters, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId, Role userRole, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {

        var whereClause = userRole switch
        {
            Role.Customer => "WHERE CustomerId = @UserId",
            Role.Owner => "WHERE OwnerId = @UserId",
            Role.Admin => "",
            _ => throw new ArgumentOutOfRangeException(nameof(userRole), $"Unsupported role: {userRole}")
        };

        const string baseSql = """
                                   SELECT *
                                   FROM [Supplies].[Rental]
                               """;

        var sql = $"{baseSql} {whereClause} ORDER BY CreatedAt DESC";

        return await _connection.QueryAsync<Rental>(
            new CommandDefinition(
                sql, new { UserId = userId },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<Rental> UpdateStatusAsync(Rental rentalToUpdate, Guid manipulator, ITransaction? transaction = null, CancellationToken cancellationToken = default)
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
                rentalToUpdate.Id,
                rentalToUpdate.UpdatedAt,
                Status = rentalToUpdate.Status.ToString(),
                rentalToUpdate.UpdatedBy,
            }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<Rental?> GetByIdWithItemsAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                                   SELECT *
                                   FROM [Supplies].[Rental]
                                   WHERE Id = @id;

                                   SELECT *
                                   FROM [Supplies].[RentalItem]
                                   WHERE RentalId = @id;
                           """;

        await using var multi = await _connection.QueryMultipleAsync(
            new CommandDefinition(sql, new { id },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));

        var rental = await multi.ReadFirstOrDefaultAsync<Rental>();
        if (rental is null) return null;

        var rentalItems = await multi.ReadAsync<RentalItem>();
        rental.RentalItems = rentalItems.ToList();
        return rental;
    }

    public async Task AddRentalItemsAsync(IEnumerable<RentalItem> items, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               INSERT INTO [Supplies].[RentalItem]
                                   (RentalId, EquipmentItemId, ActualPrice, CreatedBy)
                               VALUES
                                   (@RentalId, @EquipmentItemId, @ActualPrice, @CreatedBy)
                           """;

        // TODO maybe ZDapperPlus with _connection.BulkInsert(items); ?

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                items.Select(item => new
                {
                    item.RentalId,
                    item.EquipmentItemId,
                    item.ActualPrice,
                    item.CreatedBy,
                }),
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[Rental] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
