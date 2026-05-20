using System;

namespace WaterTracker.Core.Entities
{
    public class WaterIntakeEntry
    {
        public int Id { get; private set; }

        public string UserId { get; private set; } = null!;

        public DateTime ConsumedAtUtc { get; private set; }

        public int AmountMl { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime? LastUpdatedAtUtc { get; private set; }

        private WaterIntakeEntry()
        { 
            // required by EFC
        }

        public WaterIntakeEntry(string userId, DateTime consumedAtUtc, int amountMl)
        {
            if(string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            if (consumedAtUtc == default)
                throw new ArgumentException("Consumed date and time is required.", nameof(consumedAtUtc));

            if (amountMl <= 0)
                throw new ArgumentOutOfRangeException(nameof(amountMl), "Water intake must be greater than zero");

            UserId = userId;
            ConsumedAtUtc = consumedAtUtc;
            AmountMl = amountMl;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void Update(DateTime consumedAtUtc, int amountMl)
        {
            if (consumedAtUtc == default)
                throw new ArgumentException("Consumed date and time is required.", nameof(consumedAtUtc));

            if (amountMl <= 0)
                throw new ArgumentOutOfRangeException(nameof(amountMl), "Water intake must be greater than zero");

            ConsumedAtUtc = consumedAtUtc;
            AmountMl = amountMl;
            LastUpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
