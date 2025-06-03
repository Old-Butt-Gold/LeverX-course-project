namespace EER.Application.Features.Authentication.Commands.LoginUser;

public class UserLoggedDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public bool IsSuccess { get; init; }
}
