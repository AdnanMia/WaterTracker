using WaterTracker.Contracts.Authentication;

namespace WaterTracker.Client.Services.Authentication;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}
