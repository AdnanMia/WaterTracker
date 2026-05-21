namespace WaterTracker.Client.Services.Authentication;

public interface ITokenStorageService
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task RemoveTokenAsync();
}
