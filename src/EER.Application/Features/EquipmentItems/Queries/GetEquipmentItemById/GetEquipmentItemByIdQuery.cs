using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;

public record GetEquipmentItemByIdQuery(long Id) : IRequest<EquipmentItem?>;
