namespace EER.Application.Dto.Security.RegisterAdmin;

public record RegisterAdminDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
