namespace EER.Persistence.MongoDB.Documents.Equipment;

public class ReviewEmbedded
{
    public Guid CustomerId { get; set; }
    public byte Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
