using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Category>
{
    private readonly ICategoryRepository _repository;

    public UpdateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Category> Handle(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
            throw new KeyNotFoundException("Category with provided ID is not found");

        var updatedCategory = command.Category;

        // TODO UpdatedBy

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;
        category.Slug = updatedCategory.Slug;
        category.UpdatedBy = Guid.NewGuid();
        category.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(category, cancellationToken);
    }
}
