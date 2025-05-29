using MediatR;

namespace EER.Application.Features.Equipment.Commands.DeleteEquipment;

public record DeleteEquipmentCommand(int Id) : IRequest<bool>;
