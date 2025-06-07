using MediatR;

namespace EER.Application.Features.Equipment.Commands.DeleteEquipment;

public record DeleteEquipmentCommand(DeleteEquipmentRequestDto RequestDto) : IRequest<bool>;
