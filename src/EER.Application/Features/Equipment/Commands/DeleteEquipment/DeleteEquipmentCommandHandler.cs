using EER.Domain.DatabaseAbstractions;
using EER.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Features.Equipment.Commands.DeleteEquipment;

internal sealed class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _repository;
    private readonly ILogger<DeleteEquipmentCommandHandler> _logger;

    public DeleteEquipmentCommandHandler(IEquipmentRepository repository, ILogger<DeleteEquipmentCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteEquipmentCommand command, CancellationToken cancellationToken)
    {
        var dto = command.RequestDto;

        var equipment = await _repository.GetByIdAsync(dto.Id, cancellationToken: cancellationToken);

        if (dto.UserRole != Role.Admin && equipment is not null && equipment.OwnerId != dto.Manipulator)
        {
            _logger.LogInformation("User with {userId} tried to delete equipment with id {equipmentId} of Owner {ownerId}",
                dto.Manipulator, equipment.Id, equipment.OwnerId);

            throw new UnauthorizedAccessException("You have no access to update this equipment");
        }

        return await _repository.DeleteAsync(dto.Id, cancellationToken: cancellationToken);
    }
}
