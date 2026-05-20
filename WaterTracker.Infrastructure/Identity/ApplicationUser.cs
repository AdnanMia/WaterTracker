using Microsoft.AspNetCore.Identity;

namespace WaterTracker.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    // IdentityUser doesn't have a friendly name out of the box, so we store one here for the UI to use.
    public string DisplayName { get; set; } = string.Empty;
}
