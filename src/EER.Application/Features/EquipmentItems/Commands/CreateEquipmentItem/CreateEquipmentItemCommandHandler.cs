using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

internal sealed class CreateEquipmentItemCommandHandler : IRequestHandler<CreateEquipmentItemCommand, EquipmentItem>
{
    private readonly IEquipmentItemRepository _repository;

    public CreateEquipmentItemCommandHandler(IEquipmentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<EquipmentItem> Handle(
        CreateEquipmentItemCommand command,
        CancellationToken cancellationToken)
    {
        // TODO UpdatedBy
        var item = new EquipmentItem
        {
            EquipmentId = command.EquipmentId,
            OfficeId = command.OfficeId,
            SerialNumber = command.SerialNumber,
            ItemStatus = command.ItemStatus,
            MaintenanceDate = command.MaintenanceDate,
            PurchaseDate = command.PurchaseDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
        };

        return await _repository.AddAsync(item, cancellationToken);
    }
}
