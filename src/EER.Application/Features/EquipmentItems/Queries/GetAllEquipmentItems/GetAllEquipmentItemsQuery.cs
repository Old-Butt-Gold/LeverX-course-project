using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetAllEquipmentItems;

public record GetAllEquipmentItemsQuery : IRequest<IEnumerable<EquipmentItemDto>>;
