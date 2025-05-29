using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetAllEquipment;

internal sealed class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, IEnumerable<Domain.Entities.Equipment>>
{
    private readonly IEquipmentRepository _repository;

    public GetAllEquipmentQueryHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Domain.Entities.Equipment>> Handle(GetAllEquipmentQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
