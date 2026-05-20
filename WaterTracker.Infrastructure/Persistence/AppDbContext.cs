using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterTracker.Core.Entities;
using WaterTracker.Core.Interfaces;
using WaterTracker.Infrastructure.Identity;

namespace WaterTracker.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WaterIntakeEntry> WaterIntakeEntries => Set<WaterIntakeEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
