using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetAllEquipment;

internal sealed class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, IEnumerable<EquipmentDto>>
{
    private readonly IEquipmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAllEquipmentQueryHandler(IEquipmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EquipmentDto>> Handle(GetAllEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetAllAsync(cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<EquipmentDto>>(equipment);
    }
}
