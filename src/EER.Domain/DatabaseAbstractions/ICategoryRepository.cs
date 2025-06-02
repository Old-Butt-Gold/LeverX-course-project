using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<Category> UpdateAsync(Category category, ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
