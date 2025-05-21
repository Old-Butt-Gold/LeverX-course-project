using EER.Domain.Entities.Abstractions;
using EER.Domain.Enums;

namespace EER.Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public Role UserRole { get; set; }

    public DateTime CreatedAt { get; set; }
}
