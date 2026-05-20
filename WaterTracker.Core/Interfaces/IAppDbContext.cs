using Microsoft.EntityFrameworkCore;
using WaterTracker.Core.Entities;

namespace WaterTracker.Core.Interfaces;

public interface IAppDbContext
{
    DbSet<WaterIntakeEntry> WaterIntakeEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
