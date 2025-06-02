using EER.Domain.Enums;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public record RentalUpdatedDto
{
    public int Id { get; init; }
    public RentalStatus Status { get; init; }
    public DateTime UpdatedAt { get; init; }
}
