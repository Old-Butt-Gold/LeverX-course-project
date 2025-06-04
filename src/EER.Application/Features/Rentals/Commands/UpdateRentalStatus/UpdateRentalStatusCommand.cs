using MediatR;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public record UpdateRentalStatusCommand(UpdateRentalDto UpdateRentalDto, Guid Manipulator) : IRequest<RentalUpdatedDto>;
