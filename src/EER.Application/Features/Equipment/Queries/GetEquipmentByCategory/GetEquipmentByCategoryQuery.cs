using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetEquipmentByCategory;

public record GetEquipmentByCategoryQuery(int CategoryId) : IRequest<IEnumerable<Domain.Entities.Equipment>>;
