using EER.Domain.Enums;

namespace EER.Application.Features.Users.Commands.CreateUser;

public record CreateUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public Role UserRole { get; init; }
}
