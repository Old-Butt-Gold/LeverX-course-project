namespace EER.Application.Features.Equipment.Queries.GetEquipmentById;

public record EquipmentDetailsDto
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public Guid OwnerId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal PricePerDay { get; init; }
    public decimal AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
