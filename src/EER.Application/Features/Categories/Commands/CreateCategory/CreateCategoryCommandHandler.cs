using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Category>
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Category> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        // TODO UpdatedBy
        var category = new Category
        {
            Name = command.Name,
            Description = command.Description,
            Slug = command.Slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
        };

        return await _repository.AddAsync(category, cancellationToken);
    }
}
