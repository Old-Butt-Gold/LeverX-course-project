namespace EER.Domain.Entities;

public class Favorites
{
    public Guid UserId { get; set; }
    public int EquipmentId { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
