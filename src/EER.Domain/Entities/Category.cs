using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Category : BaseEntity<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Slug { get; set; }
    public int TotalEquipment { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
