namespace EER.API.Settings;

public class JwtSettings
{
    public const string Section = "JwtSettings";
    public string? ValidIssuer { get; init; }
    public string? ValidAudience { get; init; }
    public int ExpirySeconds { get; init; }
    public int RefreshExpirySeconds { get; init; }
    public string? SigningKey { get; init; }
}
