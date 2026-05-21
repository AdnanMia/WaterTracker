using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace WaterTracker.Client.Services.Authentication;

public sealed class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly ITokenStorageService _tokenStorage;

    public TokenAuthenticationStateProvider(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        IEnumerable<Claim> claims;
        try
        {
            claims = ParseClaimsFromJwt(token);
        }
        catch
        {
            await _tokenStorage.RemoveTokenAsync();
            return Anonymous;
        }

        if (IsTokenExpired(claims))
        {
            await _tokenStorage.RemoveTokenAsync();
            return Anonymous;
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static bool IsTokenExpired(IEnumerable<Claim> claims)
    {
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (expClaim is null || !long.TryParse(expClaim, out var expUnix))
            return true;

        var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix);
        return expiry <= DateTimeOffset.UtcNow;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length != 3)
            throw new FormatException("JWT does not have three parts.");

        var jsonBytes = Base64UrlDecode(parts[1]);
        var pairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)
            ?? throw new FormatException("JWT payload could not be deserialised.");

        var claims = new List<Claim>();
        foreach (var (key, element) in pairs)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                    claims.Add(new Claim(key, item.GetString() ?? string.Empty));
            }
            else
            {
                claims.Add(new Claim(key, element.ToString()));
            }
        }
        return claims;
    }

    private static byte[] Base64UrlDecode(string base64Url)
    {
        var s = base64Url.Replace('-', '+').Replace('_', '/');
        var pad = s.Length % 4;
        if (pad == 2) s += "==";
        else if (pad == 3) s += "=";
        return Convert.FromBase64String(s);
    }
}
