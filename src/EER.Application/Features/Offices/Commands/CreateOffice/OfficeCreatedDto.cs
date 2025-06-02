namespace EER.Application.Features.Offices.Commands.CreateOffice;

public record OfficeCreatedDto
{
    public int Id { get; init; }
    public Guid OwnerId { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
