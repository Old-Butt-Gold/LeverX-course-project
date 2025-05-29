namespace EER.Persistence.MongoDB.Documents.Office;

public class OfficeDocument
{
    public int Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public bool IsActive { get; set; }
    public List<long> EquipmentItemIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
