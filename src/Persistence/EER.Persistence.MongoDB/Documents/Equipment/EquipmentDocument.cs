using EER.Persistence.MongoDB.Documents.EquipmentItem;

namespace EER.Persistence.MongoDB.Documents.Equipment;

public class EquipmentDocument
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsModerated { get; set; }
    public List<EquipmentImageEmbedded> Images { get; set; } = [];
    public List<ReviewEmbedded> Reviews { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
