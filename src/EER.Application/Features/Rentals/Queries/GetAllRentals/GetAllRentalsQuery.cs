﻿using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetAllRentals;

public record GetAllRentalsQuery : IRequest<IEnumerable<RentalDto>>;
