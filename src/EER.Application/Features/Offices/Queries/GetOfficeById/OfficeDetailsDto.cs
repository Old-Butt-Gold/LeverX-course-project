namespace EER.Application.Features.Offices.Queries.GetOfficeById;

public record OfficeDetailsDto
{
    public int Id { get; init; }
    public Guid OwnerId { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    // TODO EquipmentItems Array
}
