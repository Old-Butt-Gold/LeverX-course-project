using MediatR;

namespace EER.Application.Features.Offices.Queries.GetAllOffices;

public record GetAllOfficesQuery : IRequest<IEnumerable<OfficeDto>>;
