using MediatR;

namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

public record CreateEquipmentCommand(
    string Name,
    int CategoryId,
    Guid OwnerId,
    string Description,
    decimal PricePerDay) : IRequest<Domain.Entities.Equipment>;
