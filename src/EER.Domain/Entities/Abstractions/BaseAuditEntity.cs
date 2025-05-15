namespace EER.Domain.Entities.Abstractions;

public abstract class BaseAuditEntity<TKey> : BaseEntity<TKey> where TKey : struct
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Later used for "Soft Delete"
    public bool IsDeleted { get; set; } = false;
}