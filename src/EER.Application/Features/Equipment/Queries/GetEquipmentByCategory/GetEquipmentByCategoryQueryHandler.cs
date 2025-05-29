using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetEquipmentByCategory;

internal sealed class GetEquipmentByCategoryQueryHandler
    : IRequestHandler<GetEquipmentByCategoryQuery, IEnumerable<Domain.Entities.Equipment>>
{
    private readonly IEquipmentRepository _repository;

    public GetEquipmentByCategoryQueryHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Domain.Entities.Equipment>> Handle(
        GetEquipmentByCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByCategoryAsync(request.CategoryId, cancellationToken);
    }
}
