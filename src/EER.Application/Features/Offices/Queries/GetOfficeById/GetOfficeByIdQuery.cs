using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Queries.GetOfficeById;

public record GetOfficeByIdQuery(int Id) : IRequest<Office?>;
