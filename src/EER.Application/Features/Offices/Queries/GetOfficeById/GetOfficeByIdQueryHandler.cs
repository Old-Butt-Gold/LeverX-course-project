using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Queries.GetOfficeById;

internal sealed class GetOfficeByIdQueryHandler : IRequestHandler<GetOfficeByIdQuery, Office?>
{
    private readonly IOfficeRepository _repository;

    public GetOfficeByIdQueryHandler(IOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Office?> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
