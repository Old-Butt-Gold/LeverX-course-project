namespace EER.Application.Features.Equipment.Queries.GetAllEquipment;

public record EquipmentDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int CategoryId { get; init; }
    public decimal PricePerDay { get; init; }
    public decimal AverageRating { get; init; }
    public bool IsModerated { get; init; }
    public DateTime CreatedAt { get; init; }
}
