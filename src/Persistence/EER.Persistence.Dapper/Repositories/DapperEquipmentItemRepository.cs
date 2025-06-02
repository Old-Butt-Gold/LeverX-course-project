using System.Data.Common;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperEquipmentItemRepository : IEquipmentItemRepository
{
    private readonly DbConnection _connection;

    public DapperEquipmentItemRepository(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[EquipmentItem]";
        return await _connection.QueryAsync<EquipmentItem>(
            new CommandDefinition(sql, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[EquipmentItem] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<EquipmentItem>(
            new CommandDefinition(sql, new { Id = id }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<EquipmentItem> AddAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               INSERT INTO [Supplies].[EquipmentItem] (
                                   EquipmentId, OfficeId, SerialNumber, ItemStatus,
                                   MaintenanceDate, PurchaseDate, CreatedBy, UpdatedBy
                               )
                               OUTPUT INSERTED.*
                               VALUES (
                                   @EquipmentId, @OfficeId, @SerialNumber, @ItemStatus,
                                   @MaintenanceDate, @PurchaseDate, @CreatedBy, @UpdatedBy
                               )
                           """;

        var parameters = new
        {
            item.EquipmentId,
            item.OfficeId,
            item.SerialNumber,
            ItemStatus = item.ItemStatus.ToString(),
            item.MaintenanceDate,
            item.PurchaseDate,
            item.CreatedBy,
            item.UpdatedBy
        };

        return await _connection.QuerySingleAsync<EquipmentItem>(
            new CommandDefinition(sql, parameters, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<EquipmentItem> UpdateAsync(EquipmentItem item, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               UPDATE [Supplies].[EquipmentItem]
                               SET
                                   OfficeId = @OfficeId,
                                   SerialNumber = @SerialNumber,
                                   ItemStatus = @ItemStatus,
                                   MaintenanceDate = @MaintenanceDate,
                                   PurchaseDate = @PurchaseDate,
                                   UpdatedBy = @UpdatedBy,
                                   UpdatedAt = @UpdatedAt
                               OUTPUT INSERTED.*
                               WHERE Id = @Id
                           """;

        var parameters = new
        {
            item.Id,
            item.OfficeId,
            item.SerialNumber,
            ItemStatus = item.ItemStatus.ToString(),
            item.MaintenanceDate,
            item.PurchaseDate,
            item.UpdatedBy,
            item.UpdatedAt
        };

        return await _connection.QuerySingleAsync<EquipmentItem>(
            new CommandDefinition(sql, parameters, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(long id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[EquipmentItem] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
