namespace WaterTracker.Core.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(string userId, string email, string displayName);
}
