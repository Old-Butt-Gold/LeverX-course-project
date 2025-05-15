using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class User : BaseAuditEntity<Guid>
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<Equipment> OwnedEquipment { get; set; } = [];
    public ICollection<Rental> Rentals { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}