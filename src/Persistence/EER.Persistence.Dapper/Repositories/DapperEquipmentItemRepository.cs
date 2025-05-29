using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperEquipmentItemRepository : IEquipmentItemRepository
{
    private readonly IDbConnection _connection;

    public DapperEquipmentItemRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[EquipmentItem]";
        return await _connection.QueryAsync<EquipmentItem>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<EquipmentItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[EquipmentItem] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<EquipmentItem>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<EquipmentItem> AddAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        // TODO CreatedBy
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
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task<EquipmentItem> UpdateAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        // TODO UpdatedBy
        const string sql = """
                               UPDATE [Supplies].[EquipmentItem]
                               SET
                                   OfficeId = @OfficeId,
                                   SerialNumber = @SerialNumber,
                                   ItemStatus = @ItemStatus,
                                   MaintenanceDate = @MaintenanceDate,
                                   PurchaseDate = @PurchaseDate,
                                   UpdatedBy = @UpdatedBy,
                                   UpdatedAt = GETUTCDATE()
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
            item.UpdatedBy
        };

        return await _connection.QuerySingleAsync<EquipmentItem>(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[EquipmentItem] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
