using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Categories.Commands.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _repository;

    public DeleteCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        // TODO can't delete category if total count of equipment of this category is > 0

        return await _repository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
    }
}
