using MediatR;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public record UpdateRentalStatusCommand(UpdateRentalDto UpdateRentalDto) : IRequest<RentalUpdatedDto>;
