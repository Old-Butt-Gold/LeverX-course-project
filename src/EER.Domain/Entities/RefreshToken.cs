using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class RefreshToken : BaseEntity<long>
{
    public Guid UserId { get; set; }

    public required string Token { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public virtual User User { get; set; } = null!;
}
