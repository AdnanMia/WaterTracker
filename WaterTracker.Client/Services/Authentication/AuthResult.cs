namespace WaterTracker.Client.Services.Authentication;

public sealed record AuthResult(bool Succeeded, string? ErrorMessage = null)
{
    public static AuthResult Success() => new(true);
    public static AuthResult Failure(string errorMessage) => new(false, errorMessage);
}
