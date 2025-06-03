namespace EER.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
