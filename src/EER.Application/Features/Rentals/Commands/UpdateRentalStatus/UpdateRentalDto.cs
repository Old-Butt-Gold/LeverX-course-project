using EER.Domain.Enums;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public record UpdateRentalDto
{
    public int Id { get; init; }
    public RentalStatus Status { get; init; }
}
