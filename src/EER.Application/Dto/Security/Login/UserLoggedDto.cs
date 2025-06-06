namespace EER.Application.Dto.Security.Login;

public class UserLoggedDto
{
    public required Guid UserId { get; init; }
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public bool IsSuccess { get; init; }
}
