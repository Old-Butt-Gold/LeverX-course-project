using EER.Domain.Enums;

namespace EER.Persistence.MongoDB.Documents.Rental;

public class RentalDocument
{
    public int Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public RentalStatus Status { get; set; }
    public List<RentalItemEmbedded> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
