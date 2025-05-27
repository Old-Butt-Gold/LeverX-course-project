using EER.Domain.Entities.Abstractions;
using EER.Domain.Enums;

namespace EER.Domain.Entities;

public class EquipmentItem : BaseEntity<long>
{
    public int EquipmentId { get; set; }
    public int? OfficeId { get; set; }
    public required string SerialNumber { get; set; }
    public ItemStatus ItemStatus { get; set; }
    public DateTime? MaintenanceDate { get; set; } // DateOnly
    public DateTime PurchaseDate { get; set; } // DateOnly

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
