using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

internal sealed class DapperOfficeRepository : IOfficeRepository
{
    private readonly IDbConnection _connection;

    public DapperOfficeRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Identity].[Office]";
        return await _connection.QueryAsync<Office>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Identity].[Office] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Office>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Office> AddAsync(Office office, CancellationToken cancellationToken = default)
    {
        // TODO CreatedBy
        const string sql = """
                               INSERT INTO [Identity].[Office] (
                                   OwnerId, Address, City, Country, CreatedBy, UpdatedBy
                               )
                               OUTPUT INSERTED.*
                               VALUES (
                                   @OwnerId, @Address, @City, @Country, @CreatedBy, @UpdatedBy
                               )
                           """;

        return await _connection.QuerySingleAsync<Office>(
            new CommandDefinition(sql, new
            {
                office.OwnerId,
                office.Address,
                office.City,
                office.Country,
                office.CreatedBy,
                office.UpdatedBy
            }, cancellationToken: cancellationToken));
    }

    public async Task<Office?> UpdateAsync(Office office, CancellationToken cancellationToken = default)
    {
        // TODO UpdatedBy
        const string sql = """
                               UPDATE [Identity].[Office]
                               SET
                                   OwnerId = @OwnerId,
                                   Address = @Address,
                                   City = @City,
                                   Country = @Country,
                                   IsActive = @IsActive,
                                   UpdatedBy = @UpdatedBy,
                                   UpdatedAt = GETUTCDATE()
                               OUTPUT INSERTED.*
                               WHERE Id = @Id
                           """;

        return await _connection.QuerySingleOrDefaultAsync<Office>(
            new CommandDefinition(sql, new
            {
                office.Id,
                office.OwnerId,
                office.Address,
                office.City,
                office.Country,
                office.IsActive,
                office.UpdatedBy
            }, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Identity].[Office] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
