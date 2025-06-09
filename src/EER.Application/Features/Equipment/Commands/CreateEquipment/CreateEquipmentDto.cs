namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

public record CreateEquipmentDto
{
    public required string Name { get; init; }
    public required int CategoryId { get; init; }
    public required string Description { get; init; }
    public required decimal PricePerDay { get; init; }
}
