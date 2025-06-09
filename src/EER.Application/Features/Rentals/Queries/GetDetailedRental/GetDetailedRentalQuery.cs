using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetDetailedRental;

public record GetDetailedRentalQuery(int Id, Guid UserId)
    : IRequest<DetailedRentalDto?>;
