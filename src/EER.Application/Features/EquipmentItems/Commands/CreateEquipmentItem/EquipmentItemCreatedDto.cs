using EER.Domain.Enums;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

public record EquipmentItemCreatedDto
{
    public long Id { get; init; }
    public required string SerialNumber { get; init; }
    public ItemStatus ItemStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}
