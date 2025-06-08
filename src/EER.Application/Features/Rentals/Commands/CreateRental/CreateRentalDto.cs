namespace EER.Application.Features.Rentals.Commands.CreateRental;

public record CreateRentalDto
{
    public required Guid CustomerId { get; init; }
    public required Guid OwnerId { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }

    public ICollection<long> EquipmentItemIds { get; init; } = null!;
}
