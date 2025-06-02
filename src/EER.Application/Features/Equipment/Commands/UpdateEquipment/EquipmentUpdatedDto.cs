namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

public record EquipmentUpdatedDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal PricePerDay { get; init; }
    public int CategoryId { get; init; }
}
