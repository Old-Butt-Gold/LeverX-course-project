namespace EER.Application.Features.Equipment.Queries.GetUnmoderatedEquipment;

public record EquipmentForModerationDto
{
    public int Id { get; init; }
    public Guid OwnerId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal PricePerDay { get; init; }
    public DateTime CreatedAt { get; init; }
}
