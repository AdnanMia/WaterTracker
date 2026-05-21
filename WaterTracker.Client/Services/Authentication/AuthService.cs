using System.Net.Http.Json;
using WaterTracker.Contracts.Authentication;

namespace WaterTracker.Client.Services.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ITokenStorageService _tokenStorage;

    public AuthService(HttpClient http, ITokenStorageService tokenStorage)
    {
        _http = http;
        _tokenStorage = tokenStorage;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return AuthResult.Failure("Invalid email or password.");

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
            if (auth is null)
                return AuthResult.Failure("An unexpected error occurred. Please try again.");

            await _tokenStorage.SetTokenAsync(auth.Token);
            return AuthResult.Success();
        }
        catch (HttpRequestException)
        {
            return AuthResult.Failure("Unable to reach the server. Please check your connection.");
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return AuthResult.Failure("Registration failed. The email may already be in use.");

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
            if (auth is null)
                return AuthResult.Failure("An unexpected error occurred. Please try again.");

            await _tokenStorage.SetTokenAsync(auth.Token);
            return AuthResult.Success();
        }
        catch (HttpRequestException)
        {
            return AuthResult.Failure("Unable to reach the server. Please check your connection.");
        }
    }

    public Task LogoutAsync() => _tokenStorage.RemoveTokenAsync();

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
