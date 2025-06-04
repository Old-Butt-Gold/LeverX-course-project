using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

public record UpdateEquipmentItemCommand(UpdateEquipmentItemDto UpdateEquipmentItemDto, Guid Manipulator)
    : IRequest<EquipmentItemUpdatedDto>;
