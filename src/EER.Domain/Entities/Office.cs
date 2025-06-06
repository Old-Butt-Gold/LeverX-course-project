using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Office : BaseEntity<int>
{
    public Guid OwnerId { get; set; }
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public virtual ICollection<EquipmentItem> EquipmentItems { get; set; } = [];

    public virtual User Owner { get; set; } = null!;
}
