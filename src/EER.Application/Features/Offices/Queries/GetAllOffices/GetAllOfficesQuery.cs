using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Queries.GetAllOffices;

public record GetAllOfficesQuery : IRequest<IEnumerable<Office>>;
