using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Commands.DeleteEquipment;

internal sealed class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _repository;

    public DeleteEquipmentCommandHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteEquipmentCommand command, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(command.Id, cancellationToken);
    }
}
