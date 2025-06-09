using EER.Domain.Enums;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetMyRentals;

public record GetMyRentalsQuery(Guid UserId, Role UserRole) : IRequest<IEnumerable<MyRentalDto>>;
