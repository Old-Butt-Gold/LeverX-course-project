using MediatR;

namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

public record CreateEquipmentCommand(CreateEquipmentDto CreateEquipmentDto)
    : IRequest<EquipmentCreatedDto>;
