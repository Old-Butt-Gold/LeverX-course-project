using EER.Application.Abstractions.Services;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly Dictionary<int, Category> _categories = [];
    private int _idCounter;

    public IEnumerable<Category> GetAll() => _categories.Values.ToList();

    public Category? GetById(int id) => _categories.GetValueOrDefault(id);

    public Category Create(Category category)
    {
        category.Id = Interlocked.Increment(ref _idCounter);
        _categories[category.Id] = category;
        return category;
    }

    public Category? Update(int id, Category updatedCategory)
    {
        if (!_categories.TryGetValue(id, out var category))
            return null;

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;
        category.Slug = updatedCategory.Slug;
        category.TotalEquipment = updatedCategory.TotalEquipment;

        return category;
    }

    public bool Delete(int id) => _categories.Remove(id);
}
