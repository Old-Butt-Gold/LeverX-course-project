namespace EER.Application.Features.Reviews.CreateReview;

public record ReviewCreatedDto
{
    public Guid CustomerId { get; init; }
    public int EquipmentId { get; init; }
    public byte Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
}
