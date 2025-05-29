using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Queries.GetAllOffices;

internal sealed class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, IEnumerable<Office>>
{
    private readonly IOfficeRepository _repository;

    public GetAllOfficesQueryHandler(IOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Office>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
