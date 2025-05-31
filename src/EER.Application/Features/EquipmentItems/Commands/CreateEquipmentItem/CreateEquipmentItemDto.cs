using EER.Domain.Enums;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

public record CreateEquipmentItemDto
{
    public required int EquipmentId { get; init; }
    public int? OfficeId { get; init; }
    public required string SerialNumber { get; init; }
    public ItemStatus ItemStatus { get; init; }
    public DateOnly? MaintenanceDate { get; init; }
    public required DateOnly PurchaseDate { get; init; }
}
