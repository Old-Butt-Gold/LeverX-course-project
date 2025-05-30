using System.Data;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

// if need can inject interface of other repositories
internal sealed class DapperCategoryRepository : ICategoryRepository
{
    private readonly IDbConnection _connection;

    public DapperCategoryRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Category]";
        return await _connection.QueryAsync<Category>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM [Supplies].[Category] WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Category>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               INSERT INTO [Supplies].[Category] (Name, Description, Slug, CreatedBy, UpdatedBy)
                               OUTPUT INSERTED.*
                               VALUES (@Name, @Description, @Slug, @CreatedBy, @UpdatedBy)
                           """;

        return await _connection.QuerySingleAsync<Category>(
            new CommandDefinition(sql, new
            {
                category.Name,
                category.Description,
                category.Slug,
                category.CreatedBy,
                category.UpdatedBy
            }, cancellationToken: cancellationToken));
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               UPDATE [Supplies].[Category]
                               SET
                                   Name = @Name,
                                   Description = @Description,
                                   Slug = @Slug,
                                   UpdatedBy = @UpdatedBy,
                                   UpdatedAt = @UpdatedAt
                               OUTPUT INSERTED.*
                               WHERE Id = @Id
                           """;

        return await _connection.QuerySingleAsync<Category>(
            new CommandDefinition(sql, new
            {
                category.Id,
                category.Name,
                category.Description,
                category.Slug,
                category.UpdatedBy,
                category.UpdatedAt,
            }, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM [Supplies].[Category] WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affectedRows > 0;
    }
}
