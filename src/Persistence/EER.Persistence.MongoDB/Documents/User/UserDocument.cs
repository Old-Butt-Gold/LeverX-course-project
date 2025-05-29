using EER.Domain.Enums;

namespace EER.Persistence.MongoDB.Documents.User;

public class UserDocument
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? FullName { get; set; }
    public Role UserRole { get; set; }
    public List<UserFavoriteEmbedded> Favorites { get; set; } = [];
    public List<int> OfficeIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
