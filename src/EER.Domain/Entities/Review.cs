namespace EER.Domain.Entities;

public class Review
{
    public Guid CustomerId { get; set; }
    public int EquipmentId { get; set; }
    public byte Rating { get; set; } // 1 - 5
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Equipment Equipment { get; set; } = null!;
}
