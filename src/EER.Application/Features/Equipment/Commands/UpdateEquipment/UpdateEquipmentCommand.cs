using MediatR;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

public record UpdateEquipmentCommand(UpdateEquipmentDto UpdateEquipmentDto) : IRequest<EquipmentUpdatedDto>;
