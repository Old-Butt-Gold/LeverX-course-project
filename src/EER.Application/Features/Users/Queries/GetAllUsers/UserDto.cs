using EER.Domain.Enums;

namespace EER.Application.Features.Users.Queries.GetAllUsers;

public record UserDto
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public string? FullName { get; init; }
    public Role UserRole { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
