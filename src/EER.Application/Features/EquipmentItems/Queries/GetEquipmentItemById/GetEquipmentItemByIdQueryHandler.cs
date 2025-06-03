using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;

internal sealed class GetEquipmentItemByIdQueryHandler : IRequestHandler<GetEquipmentItemByIdQuery, EquipmentItemDetailsDto?>
{
    private readonly IEquipmentItemRepository _repository;
    private readonly IMapper _mapper;

    public GetEquipmentItemByIdQueryHandler(IEquipmentItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EquipmentItemDetailsDto?> Handle(GetEquipmentItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        return item is null
            ? null
            : _mapper.Map<EquipmentItemDetailsDto>(item);
    }
}
