namespace EER.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenResultDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
