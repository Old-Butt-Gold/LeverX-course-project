using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.DeleteEquipmentItem;

internal sealed class DeleteEquipmentItemCommandHandler : IRequestHandler<DeleteEquipmentItemCommand, bool>
{
    private readonly IEquipmentItemRepository _repository;

    public DeleteEquipmentItemCommandHandler(IEquipmentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteEquipmentItemCommand command, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
    }
}
