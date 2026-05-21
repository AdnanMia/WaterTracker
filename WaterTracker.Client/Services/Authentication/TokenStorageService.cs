using Microsoft.JSInterop;

namespace WaterTracker.Client.Services.Authentication;

// localStorage is used for for this take-home exercise.
// In production, token storage should be reviewed carefully due to XSS and token theft risks.
public sealed class TokenStorageService : ITokenStorageService
{
    private const string TokenKey = "auth_token";
    private readonly IJSRuntime _js;

    public TokenStorageService(IJSRuntime js) => _js = js;

    public Task<string?> GetTokenAsync() =>
        _js.InvokeAsync<string?>("localStorage.getItem", TokenKey).AsTask();

    public Task SetTokenAsync(string token) =>
        _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token).AsTask();

    public Task RemoveTokenAsync() =>
        _js.InvokeVoidAsync("localStorage.removeItem", TokenKey).AsTask();
}
