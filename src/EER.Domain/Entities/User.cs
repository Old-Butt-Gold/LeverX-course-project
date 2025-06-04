using EER.Domain.Entities.Abstractions;
using EER.Domain.Enums;

namespace EER.Domain.Entities;

public class User : BaseEntity<Guid>
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? FullName { get; set; }
    public Role UserRole { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Equipment> Equipment { get; set; } = [];

    public virtual ICollection<Favorites> Favorites { get; set; } = [];

    public virtual ICollection<Office> Offices { get; set; } = [];

    public virtual ICollection<Rental> RentalCustomers { get; set; } = [];

    public virtual ICollection<Rental> RentalOwners { get; set; } = [];

    public virtual ICollection<Review> Reviews { get; set; } = [];

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
