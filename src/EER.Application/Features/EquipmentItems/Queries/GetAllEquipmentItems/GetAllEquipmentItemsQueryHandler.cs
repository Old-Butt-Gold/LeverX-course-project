using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetAllEquipmentItems;

internal sealed class GetAllEquipmentItemsQueryHandler : IRequestHandler<GetAllEquipmentItemsQuery, IEnumerable<EquipmentItemDto>>
{
    private readonly IEquipmentItemRepository _repository;
    private readonly IMapper _mapper;

    public GetAllEquipmentItemsQueryHandler(IEquipmentItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EquipmentItemDto>> Handle(GetAllEquipmentItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<EquipmentItemDto>>(items);
    }
}
