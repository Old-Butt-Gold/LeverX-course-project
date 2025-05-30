using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;

internal sealed class GetEquipmentItemByIdQueryHandler : IRequestHandler<GetEquipmentItemByIdQuery, EquipmentItem?>
{
    private readonly IEquipmentItemRepository _repository;

    public GetEquipmentItemByIdQueryHandler(IEquipmentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<EquipmentItem?> Handle(GetEquipmentItemByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
