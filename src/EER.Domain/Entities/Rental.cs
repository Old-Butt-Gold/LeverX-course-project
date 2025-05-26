using EER.Domain.Entities.Abstractions;
using EER.Domain.Enums;

namespace EER.Domain.Entities;

public class Rental : BaseEntity<int>
{
    public Guid CustomerId { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public RentalStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}

