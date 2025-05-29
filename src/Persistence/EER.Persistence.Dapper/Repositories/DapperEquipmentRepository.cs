using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperEquipmentRepository : IEquipmentRepository
{
    private readonly IDbConnection _connection;

    public DapperEquipmentRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Equipment]";
        return await _connection.QueryAsync<Equipment>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Equipment] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Equipment>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Equipment] WHERE CategoryId = @CategoryId";
        return await _connection.QueryAsync<Equipment>(
            new CommandDefinition(sql, new { CategoryId = categoryId }, cancellationToken: cancellationToken));
    }

    public async Task<Equipment> AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        // TODO CreatedAt
        const string sql = """
                               DECLARE @InsertedTable TABLE (
                                   Id INT,
                                   Name NVARCHAR(100),
                                   CategoryId INT,
                                   Description NVARCHAR(3000),
                                   PricePerDay DECIMAL(8,2),
                                   AverageRating DECIMAL(3,2),
                                   TotalReviews INT,
                                   CreatedAt DATETIME2(2),
                                   CreatedBy UNIQUEIDENTIFIER,
                                   UpdatedAt DATETIME2(2),
                                   UpdatedBy UNIQUEIDENTIFIER
                               );

                               INSERT INTO [Supplies].[Equipment] (
                                   Name, OwnerId, CategoryId, Description, PricePerDay, CreatedBy, UpdatedBy
                               )
                               OUTPUT
                                   INSERTED.Id,
                                   INSERTED.Name,
                                   INSERTED.CategoryId,
                                   INSERTED.Description,
                                   INSERTED.PricePerDay,
                                   INSERTED.AverageRating,
                                   INSERTED.TotalReviews,
                                   INSERTED.CreatedAt,
                                   INSERTED.CreatedBy,
                                   INSERTED.UpdatedAt,
                                   INSERTED.UpdatedBy
                               INTO @InsertedTable
                               VALUES (
                                   @Name, @OwnerId, @CategoryId, @Description, @PricePerDay,
                                   @CreatedBy, @UpdatedBy
                               );

                               SELECT * FROM @InsertedTable;
                           """;

        return await _connection.QuerySingleAsync<Equipment>(
            new CommandDefinition(sql, new
            {
                equipment.Name,
                equipment.OwnerId,
                equipment.CategoryId,
                equipment.Description,
                equipment.PricePerDay,
                equipment.CreatedBy,
                equipment.UpdatedBy,
            }, cancellationToken: cancellationToken));
    }

    public async Task<Equipment> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        // TODO UpdatedBy
        const string sql = """
                               DECLARE @UpdatedTable TABLE (
                                   Id INT,
                                   Name NVARCHAR(100),
                                   OwnerId UNIQUEIDENTIFIER,
                                   CategoryId INT,
                                   Description NVARCHAR(3000),
                                   PricePerDay DECIMAL(8,2),
                                   AverageRating DECIMAL(3,2),
                                   TotalReviews INT,
                                   CreatedAt DATETIME2(2),
                                   CreatedBy UNIQUEIDENTIFIER,
                                   UpdatedAt DATETIME2(2),
                                   UpdatedBy UNIQUEIDENTIFIER
                               );

                               UPDATE [Supplies].[Equipment]
                               SET
                                   Name = @Name,
                                   CategoryId = @CategoryId,
                                   Description = @Description,
                                   PricePerDay = @PricePerDay,
                                   UpdatedBy = @UpdatedBy
                               OUTPUT
                                   INSERTED.Id,
                                   INSERTED.Name,
                                   INSERTED.OwnerId,
                                   INSERTED.CategoryId,
                                   INSERTED.Description,
                                   INSERTED.PricePerDay,
                                   INSERTED.AverageRating,
                                   INSERTED.TotalReviews,
                                   INSERTED.CreatedAt,
                                   INSERTED.CreatedBy,
                                   INSERTED.UpdatedAt,
                                   INSERTED.UpdatedBy
                               INTO @UpdatedTable
                               WHERE Id = @Id;

                               SELECT * FROM @UpdatedTable;
                           """;

        return await _connection.QuerySingleAsync<Equipment>(
            new CommandDefinition(sql, new
            {
                equipment.Id,
                equipment.Name,
                equipment.CategoryId,
                equipment.Description,
                equipment.PricePerDay,
                equipment.UpdatedBy,
            }, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[Equipment] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
