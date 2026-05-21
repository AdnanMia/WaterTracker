using System.Net.Http.Headers;
using WaterTracker.Client.Services.Authentication;

namespace WaterTracker.Client.Infrastructure;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;

    public AuthHeaderHandler(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
