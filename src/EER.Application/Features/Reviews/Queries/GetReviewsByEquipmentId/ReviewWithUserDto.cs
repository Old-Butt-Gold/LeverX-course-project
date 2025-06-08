namespace EER.Application.Features.Reviews.Queries.GetReviewsByEquipmentId;

public record ReviewWithUserDto
{
    public Guid CustomerId { get; init; }
    public string CustomerFullName { get; init; } = null!;
    public byte Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
}
