using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetEquipmentById;

internal sealed class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDetailsDto?>
{
    private readonly IEquipmentRepository _repository;
    private readonly IMapper _mapper;

    public GetEquipmentByIdQueryHandler(IEquipmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EquipmentDetailsDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetByIdAsync(request.Id, cancellationToken);

        return equipment is null
            ? null
            : _mapper.Map<EquipmentDetailsDto>(equipment);
    }
}
