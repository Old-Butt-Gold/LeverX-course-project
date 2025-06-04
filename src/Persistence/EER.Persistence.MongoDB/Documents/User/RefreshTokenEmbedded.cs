namespace EER.Persistence.MongoDB.Documents.User;

public class RefreshTokenEmbedded
{
    public long Id { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
