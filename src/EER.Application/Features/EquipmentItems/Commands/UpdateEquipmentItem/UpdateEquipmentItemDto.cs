using EER.Domain.Enums;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

public record UpdateEquipmentItemDto
{
    public long Id { get; init; }
    public int? OfficeId { get; init; }
    public required string SerialNumber { get; init; }
    public ItemStatus ItemStatus { get; init; }
    public DateOnly? MaintenanceDate { get; init; }
    public required DateOnly PurchaseDate { get; init; }
}
