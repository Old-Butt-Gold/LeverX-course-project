using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public bool IsActive { get; set; } = true; // admin can hide/show category

    // for denormalization
    public int TotalEquipment { get; set; }
}
