using WaterTracker.Core.Entities;

namespace WaterTracker.Tests.Entities;

public class WaterIntakeEntryTests
{
    private const string ValidUserId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
    private static readonly DateTime ValidDate = new(2025, 1, 15, 8, 0, 0, DateTimeKind.Utc);
    private const int ValidAmount = 250;

    // constructor

    [Fact]
    public void Constructor_WithValidValues_CreatesEntry()
    {
        var entry = new WaterIntakeEntry(ValidUserId, ValidDate, ValidAmount);

        Assert.Equal(ValidUserId, entry.UserId);
        Assert.Equal(ValidDate, entry.ConsumedAtUtc);
        Assert.Equal(ValidAmount, entry.AmountMl);
        Assert.True(entry.CreatedAtUtc <= DateTime.UtcNow);
        Assert.Null(entry.LastUpdatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Constructor_WithBlankUserId_ThrowsArgumentException(string userId)
    {
        Assert.Throws<ArgumentException>(() =>
            new WaterIntakeEntry(userId, ValidDate, ValidAmount));
    }

    [Fact]
    public void Constructor_WithDefaultConsumedAtUtc_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new WaterIntakeEntry(ValidUserId, default, ValidAmount));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    public void Constructor_WithInvalidAmount_ThrowsArgumentOutOfRangeException(int amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new WaterIntakeEntry(ValidUserId, ValidDate, amount));
    }

    // update

    [Fact]
    public void Update_WithValidValues_UpdatesConsumedAtUtcAndAmountMl()
    {
        var entry = new WaterIntakeEntry(ValidUserId, ValidDate, ValidAmount);
        var newDate = ValidDate.AddHours(2);
        const int newAmount = 500;

        entry.Update(newDate, newAmount);

        Assert.Equal(newDate, entry.ConsumedAtUtc);
        Assert.Equal(newAmount, entry.AmountMl);
    }

    [Fact]
    public void Update_WithValidValues_SetsLastUpdatedAtUtc()
    {
        var entry = new WaterIntakeEntry(ValidUserId, ValidDate, ValidAmount);
        var before = DateTime.UtcNow;

        entry.Update(ValidDate.AddHours(1), 300);

        Assert.NotNull(entry.LastUpdatedAtUtc);
        Assert.True(entry.LastUpdatedAtUtc >= before);
        Assert.True(entry.LastUpdatedAtUtc <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    public void Update_WithInvalidAmount_ThrowsArgumentOutOfRangeException(int amount)
    {
        var entry = new WaterIntakeEntry(ValidUserId, ValidDate, ValidAmount);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            entry.Update(ValidDate, amount));
    }

    [Fact]
    public void Update_WithDefaultConsumedAtUtc_ThrowsArgumentException()
    {
        var entry = new WaterIntakeEntry(ValidUserId, ValidDate, ValidAmount);

        Assert.Throws<ArgumentException>(() =>
            entry.Update(default, ValidAmount));
    }
}
