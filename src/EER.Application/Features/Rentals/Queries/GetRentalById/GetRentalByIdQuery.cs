﻿using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetRentalById;

public record GetRentalByIdQuery(int Id) : IRequest<RentalDetailsDto?>;
