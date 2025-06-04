using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

public record CreateEquipmentItemCommand(CreateEquipmentItemDto CreateEquipmentItemDto, Guid Manipulator)
    : IRequest<EquipmentItemCreatedDto>;
