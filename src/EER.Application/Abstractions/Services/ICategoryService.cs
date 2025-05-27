using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface ICategoryService
{
    IEnumerable<Category> GetAll();
    Category? GetById(int id);
    Category Create(Category category);
    Category? Update(int id, Category updatedCategory);
    bool Delete(int id);
}
