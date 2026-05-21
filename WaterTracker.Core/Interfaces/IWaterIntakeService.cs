using WaterTracker.Core.Entities;

namespace WaterTracker.Core.Interfaces;

public interface IWaterIntakeService
{
    Task<IReadOnlyList<WaterIntakeEntry>> GetForUserAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<WaterIntakeEntry?> GetForUserByIdAsync(
        string userId,
        int id,
        CancellationToken cancellationToken = default);

    Task<WaterIntakeEntry> CreateForUserAsync(
        string userId,
        DateTime consumedAtUtc,
        int amountMl,
        CancellationToken cancellationToken = default);

    Task<WaterIntakeEntry?> UpdateForUserAsync(
        string userId,
        int id,
        DateTime consumedAtUtc,
        int amountMl,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteForUserAsync(
        string userId,
        int id,
        CancellationToken cancellationToken = default);
}
