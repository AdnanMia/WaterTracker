using System.ComponentModel.DataAnnotations;

namespace WaterTracker.Contracts.Authentication;

public sealed record RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; init; } = string.Empty;
}
