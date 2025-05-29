using EER.Domain.Entities;
using EER.Domain.Enums;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public record UpdateRentalStatusCommand(
    int Id,
    RentalStatus Status) : IRequest<Rental>;
