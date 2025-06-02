using EER.Domain.Enums;

namespace EER.Application.Features.Users.Commands.CreateUser;

public record UserCreatedDto
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public Role UserRole { get; init; }
    public DateTime CreatedAt { get; init; }
}
