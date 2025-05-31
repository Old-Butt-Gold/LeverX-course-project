using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

public record UpdateEquipmentItemCommand(UpdateEquipmentItemDto UpdateEquipmentItemDto)
    : IRequest<EquipmentItemUpdatedDto>;
