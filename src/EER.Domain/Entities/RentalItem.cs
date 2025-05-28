namespace EER.Domain.Entities;

public class RentalItem
{
    public int RentalId { get; set; }
    public long EquipmentItemId { get; set; }
    public decimal ActualPrice { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public virtual EquipmentItem EquipmentItem { get; set; } = null!;

    public virtual Rental Rental { get; set; } = null!;
}
