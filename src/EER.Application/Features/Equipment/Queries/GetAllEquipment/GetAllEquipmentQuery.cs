using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetAllEquipment;

public record GetAllEquipmentQuery : IRequest<IEnumerable<Domain.Entities.Equipment>>;
