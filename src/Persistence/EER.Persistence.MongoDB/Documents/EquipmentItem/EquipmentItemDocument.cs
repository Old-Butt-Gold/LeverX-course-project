using EER.Domain.Enums;

namespace EER.Persistence.MongoDB.Documents.EquipmentItem;

public class EquipmentItemDocument
{
    public long Id { get; set; }
    public int EquipmentId { get; set; }
    public int? OfficeId { get; set; }
    public required string SerialNumber { get; set; }
    public ItemStatus Status { get; set; }
    public DateOnly? MaintenanceDate { get; set; }
    public DateOnly PurchaseDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
