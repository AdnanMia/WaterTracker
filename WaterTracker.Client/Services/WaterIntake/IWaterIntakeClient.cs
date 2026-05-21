using WaterTracker.Contracts.WaterIntake;

namespace WaterTracker.Client.Services.WaterIntake;

public interface IWaterIntakeClient
{
    Task<IReadOnlyList<WaterIntakeResponse>> GetEntriesAsync(CancellationToken cancellationToken = default);
    Task<WaterIntakeResponse?> GetEntryAsync(int id, CancellationToken cancellationToken = default);
    Task<WaterIntakeResponse?> CreateEntryAsync(LogWaterIntakeRequest request, CancellationToken cancellationToken = default);
    Task<WaterIntakeResponse?> UpdateEntryAsync(int id, UpdateWaterIntakeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteEntryAsync(int id, CancellationToken cancellationToken = default);
}
