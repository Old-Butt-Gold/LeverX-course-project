namespace EER.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
