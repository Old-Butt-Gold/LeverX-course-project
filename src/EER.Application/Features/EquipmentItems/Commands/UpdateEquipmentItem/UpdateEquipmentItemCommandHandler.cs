using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

internal sealed class UpdateEquipmentItemCommandHandler
    : IRequestHandler<UpdateEquipmentItemCommand, EquipmentItem>
{
    private readonly IEquipmentItemRepository _repository;

    public UpdateEquipmentItemCommandHandler(IEquipmentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<EquipmentItem> Handle(UpdateEquipmentItemCommand command, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (item is null)
            throw new KeyNotFoundException("EquipmentItem with provided ID is not found");

        var updatedEquipmentItem = command.EquipmentItem;

        // TODO UpdatedBy

        item.EquipmentId = updatedEquipmentItem.EquipmentId;
        item.OfficeId = updatedEquipmentItem.OfficeId;
        item.SerialNumber = updatedEquipmentItem.SerialNumber;
        item.ItemStatus = updatedEquipmentItem.ItemStatus;
        item.MaintenanceDate = updatedEquipmentItem.MaintenanceDate;
        item.PurchaseDate = updatedEquipmentItem.PurchaseDate;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = Guid.NewGuid();

        return await _repository.UpdateAsync(item, cancellationToken);
    }
}
