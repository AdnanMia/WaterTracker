namespace WaterTracker.API.Configuration;

public sealed class JwtSettings
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    // from appsettings.Development.json locally or environment variable or secrets manager.
    public string SigningKey { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 60;
}
