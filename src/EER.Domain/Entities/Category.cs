using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int TotalEquipment { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public virtual ICollection<Equipment> Equipment { get; set; } = [];
}
