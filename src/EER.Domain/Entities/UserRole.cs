using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class UserRole : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}