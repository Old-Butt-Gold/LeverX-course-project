using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Equipment : BaseEntity<int>
{
    public int CategoryId { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsModerated { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<EquipmentImages> EquipmentImages { get; set; } = [];

    public virtual ICollection<EquipmentItem> EquipmentItems { get; set; } = [];

    public virtual ICollection<Favorites> Favorites { get; set; } = [];

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = [];
}
