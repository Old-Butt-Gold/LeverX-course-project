using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.DeleteEquipmentItem;

public record DeleteEquipmentItemCommand(long Id) : IRequest<bool>;
