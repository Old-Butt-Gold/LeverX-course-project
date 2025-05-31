using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetEquipmentById;

public record GetEquipmentByIdQuery(int Id) : IRequest<EquipmentDetailsDto?>;
