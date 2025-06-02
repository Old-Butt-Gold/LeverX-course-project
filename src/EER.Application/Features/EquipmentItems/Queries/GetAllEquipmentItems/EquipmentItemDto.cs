using EER.Domain.Enums;

namespace EER.Application.Features.EquipmentItems.Queries.GetAllEquipmentItems;

public record EquipmentItemDto
{
    public long Id { get; init; }
    public int EquipmentId { get; init; }
    public int? OfficeId { get; init; }
    public required string SerialNumber { get; init; }
    public ItemStatus ItemStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}
