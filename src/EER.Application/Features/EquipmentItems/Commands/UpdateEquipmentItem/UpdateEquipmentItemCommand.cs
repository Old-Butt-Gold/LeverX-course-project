using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

public record UpdateEquipmentItemCommand(
    long Id,
    EquipmentItem EquipmentItem) : IRequest<EquipmentItem>;
