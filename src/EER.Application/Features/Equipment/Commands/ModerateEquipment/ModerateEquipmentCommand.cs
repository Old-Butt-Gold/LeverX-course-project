using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Commands.ModerateEquipment;

public record ModerateEquipmentCommand(int EquipmentId, Guid ModeratorId)
    : IRequest<bool>;

internal sealed class ModerateEquipmentCommandHandler
    : IRequestHandler<ModerateEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _repository;

    public ModerateEquipmentCommandHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ModerateEquipmentCommand command, CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetByIdAsync(command.EquipmentId, cancellationToken: cancellationToken);

        if (equipment is null)
            return false;

        equipment.IsModerated = true;
        equipment.UpdatedBy = command.ModeratorId;
        equipment.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(equipment, cancellationToken: cancellationToken);

        return true;
    }
}
