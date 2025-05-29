using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Persistence.MongoDB.Repositories;

public class MongoCategoryRepository : ICategoryRepository
{
    public Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Category?> UpdateAsync(Category category, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
