using EER.Domain.Entities.Abstractions;
using EER.Domain.Enums;

namespace EER.Domain.Entities;

public class Rental : BaseEntity<long>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public RentalStatus Status { get; set; }

    public Guid UserId { get; set; }

    public long EquipmentId { get; set; }

    public long? ReviewId { get; set; }
}