using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Offices.Queries.GetOfficeById;

internal sealed class GetOfficeByIdQueryHandler : IRequestHandler<GetOfficeByIdQuery, OfficeDetailsDto?>
{
    private readonly IOfficeRepository _repository;
    private readonly IMapper _mapper;

    public GetOfficeByIdQueryHandler(IOfficeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OfficeDetailsDto?> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
    {
        var office = await _repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        return office is null
            ? null
            : _mapper.Map<OfficeDetailsDto>(office);
    }
}
