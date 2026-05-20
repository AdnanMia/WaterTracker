namespace WaterTracker.Contracts.Authentication;

public sealed record AuthResponse(
    string UserId,
    string Email,
    string DisplayName,
    string Token,
    DateTime ExpiresAtUtc
);
