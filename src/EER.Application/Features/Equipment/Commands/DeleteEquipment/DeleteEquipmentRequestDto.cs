using EER.Domain.Enums;

namespace EER.Application.Features.Equipment.Commands.DeleteEquipment;

public record DeleteEquipmentRequestDto
{
    public int Id { get; init; }
    public Guid Manipulator { get; init; }
    public Role UserRole { get; init; }
}
