namespace EER.Persistence.MongoDB.Documents.Rental;

public class RentalItemEmbedded
{
    public long EquipmentItemId { get; set; }
    public decimal ActualPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
