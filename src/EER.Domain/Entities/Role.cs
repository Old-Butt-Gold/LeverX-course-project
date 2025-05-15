using EER.Domain.Entities.Abstractions;
using EER.Domain.ValueObjects;

namespace EER.Domain.Entities;

public class Role : BaseEntity<int>
{
    public RoleName Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
}