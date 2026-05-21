using System.Net.Http.Json;
using WaterTracker.Contracts.WaterIntake;

namespace WaterTracker.Client.Services.WaterIntake;

public sealed class WaterIntakeClient : IWaterIntakeClient
{
    private readonly HttpClient _http;

    public WaterIntakeClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<WaterIntakeResponse>> GetEntriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<WaterIntakeResponse>>("api/water-intake", cancellationToken)
                ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
    }

    public async Task<WaterIntakeResponse?> GetEntryAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<WaterIntakeResponse>($"api/water-intake/{id}", cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<WaterIntakeResponse?> CreateEntryAsync(LogWaterIntakeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/water-intake", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<WaterIntakeResponse>(cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<WaterIntakeResponse?> UpdateEntryAsync(int id, UpdateWaterIntakeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/water-intake/{id}", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<WaterIntakeResponse>(cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<bool> DeleteEntryAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/water-intake/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
