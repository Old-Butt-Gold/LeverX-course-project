using EER.Domain.Enums;

namespace EER.Application.Dto.Security.RegisterUser;

public record UserRegisteredDto
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public Role UserRole { get; init; }
    public DateTime CreatedAt { get; init; }
}
