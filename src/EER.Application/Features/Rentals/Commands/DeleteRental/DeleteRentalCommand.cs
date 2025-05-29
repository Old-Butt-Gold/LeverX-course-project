using MediatR;

namespace EER.Application.Features.Rentals.Commands.DeleteRental;

public record DeleteRentalCommand(int Id) : IRequest<bool>;
