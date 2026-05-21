using Microsoft.EntityFrameworkCore;
using WaterTracker.Core.Entities;
using WaterTracker.Core.Interfaces;

namespace WaterTracker.Infrastructure.Services;

public sealed class WaterIntakeService : IWaterIntakeService
{
    private readonly IAppDbContext _context;

    public WaterIntakeService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WaterIntakeEntry>> GetForUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.WaterIntakeEntries
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.ConsumedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<WaterIntakeEntry?> GetForUserByIdAsync(
        string userId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.WaterIntakeEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);
    }

    public async Task<WaterIntakeEntry> CreateForUserAsync(
        string userId,
        DateTime consumedAtUtc,
        int amountMl,
        CancellationToken cancellationToken = default)
    {
        var entry = new WaterIntakeEntry(userId, consumedAtUtc, amountMl);
        _context.WaterIntakeEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<WaterIntakeEntry?> UpdateForUserAsync(
        string userId,
        int id,
        DateTime consumedAtUtc,
        int amountMl,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.WaterIntakeEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);

        if (entry is null)
            return null;

        entry.Update(consumedAtUtc, amountMl);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<bool> DeleteForUserAsync(
        string userId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.WaterIntakeEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);

        if (entry is null)
            return false;

        _context.WaterIntakeEntries.Remove(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
