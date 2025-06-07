using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetUnmoderatedEquipment;

public record GetUnmoderatedEquipmentQuery : IRequest<IEnumerable<EquipmentForModerationDto>>;

internal sealed class GetUnmoderatedEquipmentQueryHandler
    : IRequestHandler<GetUnmoderatedEquipmentQuery, IEnumerable<EquipmentForModerationDto>>
{
    private readonly IEquipmentRepository _repository;
    private readonly IMapper _mapper;

    public GetUnmoderatedEquipmentQueryHandler(IEquipmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EquipmentForModerationDto>> Handle(GetUnmoderatedEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetUnmoderatedAsync(cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<EquipmentForModerationDto>>(equipment);
    }
}
