using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperEquipmentRepository : IEquipmentRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperEquipmentRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Equipment]";
        return await _connection.QueryAsync<Equipment>(
            new CommandDefinition(sql, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Equipment] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Equipment>(
            new CommandDefinition(sql, new { Id = id }, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Equipment] WHERE CategoryId = @CategoryId";
        return await _connection.QueryAsync<Equipment>(
            new CommandDefinition(sql, new { CategoryId = categoryId }, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<Equipment> AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        // TODO CreatedAt
        const string sql = """
                               INSERT INTO [Supplies].[Equipment] (
                                   Name, CategoryId, Description, PricePerDay,
                                   CreatedBy, UpdatedBy
                               )
                               OUTPUT INSERTED.*
                               VALUES (
                                   @Name, @CategoryId, @Description, @PricePerDay,
                                   @CreatedBy, @UpdatedBy
                               )
                           """;

        return await _connection.QuerySingleAsync<Equipment>(
            new CommandDefinition(sql, new
            {
                equipment.Name,
                equipment.CategoryId,
                equipment.Description,
                equipment.PricePerDay,
                equipment.CreatedBy,
                equipment.UpdatedBy,
            }, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<Equipment?> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        // TODO UpdatedBy
        const string sql = """
                               UPDATE [Supplies].[Equipment]
                               SET
                                   Name = @Name,
                                   CategoryId = @CategoryId,
                                   Description = @Description,
                                   PricePerDay = @PricePerDay,
                                   UpdatedBy = @UpdatedBy,
                                   UpdatedAt = GETUTCDATE(),
                               OUTPUT INSERTED.*
                               WHERE Id = @Id
                           """;

        return await _connection.QuerySingleOrDefaultAsync<Equipment>(
            new CommandDefinition(sql, new
            {
                equipment.Id,
                equipment.Name,
                equipment.CategoryId,
                equipment.Description,
                equipment.PricePerDay,
                equipment.UpdatedBy,
            }, transaction: _transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[Equipment] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, transaction: _transaction, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
