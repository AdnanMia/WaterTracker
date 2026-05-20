using System.ComponentModel.DataAnnotations;

namespace WaterTracker.Contracts.WaterIntake;

public sealed record LogWaterIntakeRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be between 1ml and 5000ml.")]
    public int AmountMl { get; init; }

    [Required]
    public DateTime ConsumedAtUtc { get; init; }
}
