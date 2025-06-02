using MediatR;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

public record CreateRentalCommand(CreateRentalDto CreateRentalDto)
    : IRequest<RentalCreatedDto>;
