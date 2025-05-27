using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    public CategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.CategoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.CategoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        return await _uow.CategoryRepository.AddAsync(category, cancellationToken);
    }

    public async Task<Category?> UpdateAsync(int id, Category updatedCategory, CancellationToken cancellationToken = default)
    {
        var existingCategory = await _uow.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (existingCategory is null) return null;

        existingCategory.Name = updatedCategory.Name;
        existingCategory.Description = updatedCategory.Description;
        existingCategory.Slug = updatedCategory.Slug;
        existingCategory.UpdatedBy = updatedCategory.UpdatedBy;

        return await _uow.CategoryRepository.UpdateAsync(existingCategory, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.CategoryRepository.DeleteAsync(id, cancellationToken);
    }
}
