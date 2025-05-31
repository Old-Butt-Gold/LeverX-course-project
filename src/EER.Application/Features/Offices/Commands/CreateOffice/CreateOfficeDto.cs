namespace EER.Application.Features.Offices.Commands.CreateOffice;

public record CreateOfficeDto
{
    public Guid OwnerId { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
}
