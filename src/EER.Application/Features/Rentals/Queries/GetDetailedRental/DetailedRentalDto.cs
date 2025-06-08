using EER.Domain.Enums;

namespace EER.Application.Features.Rentals.Queries.GetDetailedRental;

public record DetailedRentalDto
{
    public int Id { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal TotalPrice { get; init; }
    public RentalStatus Status { get; init; }
    public IEnumerable<DetailedEquipmentItemDto> Items { get; init; } = [];
}
