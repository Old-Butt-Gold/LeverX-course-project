using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.AddAsync(category, cancellationToken);
    }

    public async Task<Category> UpdateAsync(int id, Category updatedCategory, CancellationToken cancellationToken = default)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (existingCategory is null)
            throw new KeyNotFoundException("Category with provided ID is not found");

        existingCategory.Name = updatedCategory.Name;
        existingCategory.Description = updatedCategory.Description;
        existingCategory.Slug = updatedCategory.Slug;
        existingCategory.UpdatedBy = updatedCategory.UpdatedBy;

        return await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.DeleteAsync(id, cancellationToken);
    }
}
