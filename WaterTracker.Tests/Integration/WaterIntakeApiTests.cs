using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WaterTracker.Contracts.Authentication;
using WaterTracker.Contracts.WaterIntake;

namespace WaterTracker.Tests.Integration;

public class WaterIntakeApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WaterIntakeApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/water-intake");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ForAnotherUsersEntry_ReturnsNotFound()
    {
        // User A registers, logs in, creates an entry
        var tokenA = await RegisterAndLoginAsync($"test@test.com");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);

        var createResponse = await _client.PostAsJsonAsync("/api/water-intake", new LogWaterIntakeRequest
        {
            AmountMl = 250,
            ConsumedAtUtc = DateTime.UtcNow
        });

        var created = await createResponse.Content.ReadFromJsonAsync<WaterIntakeResponse>();
        Assert.NotNull(created);

        // User B registers and logs in
        var tokenB = await RegisterAndLoginAsync("userb@test.com");

        // User B attempts to access User A's entry, should get not found
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var response = await _client.GetAsync($"/api/water-intake/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<string> RegisterAndLoginAsync(string email, string password = "Password1")
    {
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            DisplayName = "Test User"
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }
}
