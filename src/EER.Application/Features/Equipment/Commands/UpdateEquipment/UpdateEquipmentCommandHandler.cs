using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

internal sealed class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, Domain.Entities.Equipment>
{
    private readonly IEquipmentRepository _repository;

    public UpdateEquipmentCommandHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.Equipment> Handle(
        UpdateEquipmentCommand command,
        CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (equipment is null)
            throw new KeyNotFoundException("Equipment with provided ID is not found");

        var updatedEquipment = command.Equipment;

        // TODO UpdatedBy

        equipment.Name = updatedEquipment.Name;
        equipment.CategoryId = updatedEquipment.CategoryId;
        equipment.Description = updatedEquipment.Description;
        equipment.PricePerDay = updatedEquipment.PricePerDay;
        equipment.UpdatedAt = DateTime.UtcNow;
        equipment.UpdatedBy = Guid.NewGuid();

        return await _repository.UpdateAsync(equipment, cancellationToken);
    }
}
