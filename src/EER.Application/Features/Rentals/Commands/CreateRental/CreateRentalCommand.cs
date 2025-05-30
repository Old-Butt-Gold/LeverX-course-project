using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

public record CreateRentalCommand(
    Guid OwnerId,
    Guid CustomerId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice) : IRequest<Rental>;
