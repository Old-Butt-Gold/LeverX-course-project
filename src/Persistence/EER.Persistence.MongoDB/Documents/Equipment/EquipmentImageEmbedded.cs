namespace EER.Persistence.MongoDB.Documents.Equipment;

public class EquipmentImageEmbedded
{
    public int Id { get; set; }
    public int DisplayOrder { get; set; }
    public required string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
