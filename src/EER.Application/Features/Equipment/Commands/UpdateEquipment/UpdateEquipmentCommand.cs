using MediatR;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

public record UpdateEquipmentCommand(
    int Id, Domain.Entities.Equipment Equipment) : IRequest<Domain.Entities.Equipment>;
