namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

public record EquipmentCreatedDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int CategoryId { get; init; }
    public Guid OwnerId { get; init; }
    public DateTime CreatedAt { get; init; }
}
