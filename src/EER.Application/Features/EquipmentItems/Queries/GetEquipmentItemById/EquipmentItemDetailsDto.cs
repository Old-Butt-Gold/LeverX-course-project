using EER.Domain.Enums;

namespace EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;

public record EquipmentItemDetailsDto
{
    public long Id { get; init; }
    public int EquipmentId { get; init; }
    public int? OfficeId { get; init; }
    public required string SerialNumber { get; init; }
    public ItemStatus ItemStatus { get; init; }
    public DateTime? MaintenanceDate { get; init; } // DateOnly
    public DateTime PurchaseDate { get; init; } // DateOnly
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
