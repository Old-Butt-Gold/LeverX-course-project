namespace EER.Application.Dto.Security.RefreshToken;

public class RefreshTokenDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
