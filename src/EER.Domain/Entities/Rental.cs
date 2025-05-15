using EER.Domain.Entities.Abstractions;
using EER.Domain.Enums;

namespace EER.Domain.Entities;

public class Rental : BaseEntity<long>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int Quantity { get; set; }
    public RentalStatus Status { get; set; }

    public Guid UserId { get; set; }
    public User Customer { get; set; }

    public long EquipmentId { get; set; }
    public Equipment Equipment { get; set; }

    public Review? Review { get; set; }
    
    public void CalculateTotalPrice()
    {
        var days = (EndDate - StartDate).Days + 1;
        TotalPrice = days * Equipment.PricePerDay * Quantity;
    }
}