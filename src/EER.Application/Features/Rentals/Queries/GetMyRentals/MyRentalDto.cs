using EER.Domain.Enums;

namespace EER.Application.Features.Rentals.Queries.GetMyRentals;

public record MyRentalDto
{
    public int Id { get; init; }
    public Guid CustomerId { get; init; }
    public Guid OwnerId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal TotalPrice { get; init; }
    public RentalStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
