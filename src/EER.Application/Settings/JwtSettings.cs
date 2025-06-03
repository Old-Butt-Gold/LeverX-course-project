namespace EER.Application.Settings;

public class JwtSettings
{
    public const string Section = "JwtSettings";
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public int ExpirySeconds { get; init; }
    public int RefreshExpirySeconds { get; init; }
    public string SigningKey { get; init; } = null!;
}
