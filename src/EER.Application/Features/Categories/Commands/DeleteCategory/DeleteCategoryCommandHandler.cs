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
        return await _repository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
    }
}
