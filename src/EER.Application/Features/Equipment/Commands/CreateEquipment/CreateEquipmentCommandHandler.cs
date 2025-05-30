using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

internal sealed class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Domain.Entities.Equipment>
{
    private readonly IEquipmentRepository _repository;

    public CreateEquipmentCommandHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.Equipment> Handle(CreateEquipmentCommand command, CancellationToken cancellationToken)
    {
        // TODO UpdatedBy
        var equipment = new Domain.Entities.Equipment
        {
            Name = command.Name,
            CategoryId = command.CategoryId,
            OwnerId = command.OwnerId,
            Description = command.Description,
            PricePerDay = command.PricePerDay,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            IsModerated = false,
        };

        return await _repository.AddAsync(equipment, cancellationToken);
    }
}
