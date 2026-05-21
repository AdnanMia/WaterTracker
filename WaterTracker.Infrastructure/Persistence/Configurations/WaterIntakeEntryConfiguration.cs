using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterTracker.Core.Entities;

namespace WaterTracker.Infrastructure.Persistence.Configurations;

internal sealed class WaterIntakeEntryConfiguration : IEntityTypeConfiguration<WaterIntakeEntry>
{
    public void Configure(EntityTypeBuilder<WaterIntakeEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.AmountMl)
            .IsRequired();

        builder.Property(x => x.ConsumedAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // all queries filter by UserId, index avoids a full table scan per user request
        builder.HasIndex(x => x.UserId);
    }
}
