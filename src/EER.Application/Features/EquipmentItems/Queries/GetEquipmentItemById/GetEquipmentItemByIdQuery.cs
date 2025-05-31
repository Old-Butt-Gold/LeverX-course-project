using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;

public record GetEquipmentItemByIdQuery(long Id) : IRequest<EquipmentItemDetailsDto?>;
