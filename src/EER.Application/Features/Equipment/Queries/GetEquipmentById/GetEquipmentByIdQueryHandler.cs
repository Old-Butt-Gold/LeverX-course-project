using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Queries.GetEquipmentById;

internal sealed class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, Domain.Entities.Equipment?>
{
    private readonly IEquipmentRepository _repository;

    public GetEquipmentByIdQueryHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.Equipment?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
