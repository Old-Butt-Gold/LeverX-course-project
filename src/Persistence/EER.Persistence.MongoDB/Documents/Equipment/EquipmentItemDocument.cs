using EER.Domain.Enums;

namespace EER.Persistence.MongoDB.Documents.Equipment;

public class EquipmentItemDocument
{
    public long Id { get; set; }
    public int EquipmentId { get; set; }
    public int? OfficeId { get; set; }
    public required string SerialNumber { get; set; }
    public ItemStatus Status { get; set; }
    public DateTime? MaintenanceDate { get; set; }
    public DateTime PurchaseDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
