namespace EER.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserDto
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public string? FullName { get; init; }
}
