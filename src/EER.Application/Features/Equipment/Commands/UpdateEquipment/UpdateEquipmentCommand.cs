using MediatR;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

public record UpdateEquipmentCommand(UpdateEquipmentDto UpdateEquipmentDto, Guid Manipulator) : IRequest<EquipmentUpdatedDto>;
