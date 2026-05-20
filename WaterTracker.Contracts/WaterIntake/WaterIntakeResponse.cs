namespace WaterTracker.Contracts.WaterIntake;

public sealed record WaterIntakeResponse(
    int Id,
    int AmountMl,
    DateTime ConsumedAtUtc,
    DateTime CreatedAtUtc,
    DateTime? LastUpdatedAtUtc
);
