using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Offices.Queries.GetAllOffices;

internal sealed class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, IEnumerable<OfficeDto>>
{
    private readonly IOfficeRepository _repository;
    private readonly IMapper _mapper;

    public GetAllOfficesQueryHandler(IOfficeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OfficeDto>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var offices = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<OfficeDto>>(offices);
    }
}
