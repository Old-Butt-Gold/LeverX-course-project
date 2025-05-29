namespace EER.Persistence.MongoDB.Documents.User;

public class UserFavoriteEmbedded
{
    public int EquipmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
