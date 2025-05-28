using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class EquipmentImages : BaseEntity<int>
{
    public int EquipmentId { get; set; }
    public int DisplayOrder { get; set; }
    public required string ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;
}
