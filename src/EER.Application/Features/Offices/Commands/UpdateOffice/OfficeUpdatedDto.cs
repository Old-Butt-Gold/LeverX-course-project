namespace EER.Application.Features.Offices.Commands.UpdateOffice;

public record OfficeUpdatedDto
{
    public int Id { get; init; }
    public Guid OwnerId { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public bool IsActive { get; init; }
}
