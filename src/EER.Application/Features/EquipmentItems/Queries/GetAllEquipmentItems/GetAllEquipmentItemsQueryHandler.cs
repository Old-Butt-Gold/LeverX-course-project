using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetAllEquipmentItems;

internal sealed class GetAllEquipmentItemsQueryHandler : IRequestHandler<GetAllEquipmentItemsQuery, IEnumerable<EquipmentItem>>
{
    private readonly IEquipmentItemRepository _repository;

    public GetAllEquipmentItemsQueryHandler(IEquipmentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EquipmentItem>> Handle(GetAllEquipmentItemsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
