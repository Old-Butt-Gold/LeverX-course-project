using System.Data.Common;
using Dapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Persistence.Dapper.Repositories;

public class DapperReviewRepository : IReviewRepository
{
    private readonly DbConnection _connection;

    public DapperReviewRepository(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Review> AddAsync(Review review, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               DECLARE @InsertedTable TABLE (
                                   CustomerId UNIQUEIDENTIFIER,
                                   EquipmentId INT,
                                   Rating TINYINT,
                                   Comment NVARCHAR(1000),
                                   CreatedAt DATETIME2(2),
                                   CreatedBy UNIQUEIDENTIFIER,
                                   UpdatedAt DATETIME2(2),
                                   UpdatedBy UNIQUEIDENTIFIER
                               );

                               INSERT INTO [Critique].[Review] (
                                   CustomerId, EquipmentId, Rating, Comment, CreatedBy, UpdatedBy
                               )
                               OUTPUT
                                   INSERTED.CustomerId,
                                   INSERTED.EquipmentId,
                                   INSERTED.Rating,
                                   INSERTED.Comment,
                                   INSERTED.CreatedAt,
                                   INSERTED.CreatedBy,
                                   INSERTED.UpdatedAt,
                                   INSERTED.UpdatedBy
                               INTO @InsertedTable
                               VALUES (
                                   @CustomerId, @EquipmentId, @Rating, @Comment, @CreatedBy, @UpdatedBy
                               );

                               SELECT * FROM @InsertedTable;
                           """;

        return await _connection.QuerySingleAsync<Review>(
            new CommandDefinition(sql, new
            {
                review.CustomerId,
                review.EquipmentId,
                review.Rating,
                review.Comment,
                review.CreatedBy,
                review.UpdatedBy,
            }, transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> IsExistsReview(Guid customerId, int equipmentId, ITransaction? transaction = null, CancellationToken ct = default)
    {
        const string sql = """
                               SELECT COUNT(1)
                               FROM [Critique].[Review]
                               WHERE CustomerId = @CustomerId
                               AND EquipmentId = @EquipmentId
                           """;

        var count = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new { CustomerId = customerId, EquipmentId = equipmentId },
                transaction: (transaction as DapperTransactionManager.DapperTransaction)?.Transaction,
                cancellationToken: ct
            )
        );

        return count > 0;
    }
}
