using EER.Domain.Enums;

namespace EER.Application.Dto.Security.RegisterUser;

public record RegisterUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public Role UserRole { get; init; }
}
