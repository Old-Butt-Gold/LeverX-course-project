using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Office : BaseEntity<int>
{
    public required Guid OwnerId { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public virtual ICollection<EquipmentItem> EquipmentItems { get; set; } = [];

    public virtual User Owner { get; set; } = null!;
}
