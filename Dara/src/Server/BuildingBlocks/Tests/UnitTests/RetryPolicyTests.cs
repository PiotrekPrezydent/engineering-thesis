using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

namespace Dara.Server.BuildingBlocks.UnitTests;

public class RetryPolicyTests
{
    private static readonly RetryPolicy Policy = new(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 4)]
    [InlineData(9, 256)]
    public void DelayDoublesAfterEveryFailure(int failedAttempts, int expectedSeconds)
    {
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), Policy.GetDelay(failedAttempts));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void DelayStopsAtTheMaximum(int failedAttempts)
    {
        Assert.Equal(TimeSpan.FromMinutes(5), Policy.GetDelay(failedAttempts));
    }

    [Fact]
    public void FailureCountsTheAttemptKeepsTheErrorAndMovesTheNextAttempt()
    {
        var now = new DateTime(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
        var command = new InternalCommand(Guid.NewGuid(), now, "Type", "{}", null, null);

        command.RecordFailure(new InvalidOperationException("first"), now, Policy);
        command.RecordFailure(new InvalidOperationException(new string('x', 5000)), now, Policy);

        Assert.Equal(2, command.Attempts);
        Assert.Equal(now + TimeSpan.FromSeconds(2), command.NextAttemptAt);
        Assert.StartsWith("System.InvalidOperationException: xxx", command.LastError);
        Assert.Equal(RetryableMessage.MaxErrorLength, command.LastError!.Length);
        Assert.Null(command.ProcessedDate);
    }

    [Fact]
    public void NewMessageIsReadyAtOnce()
    {
        var now = DateTime.UtcNow;
        var command = new InternalCommand(Guid.NewGuid(), now, "Type", "{}", null, null);

        Assert.Equal(now, command.NextAttemptAt);
        Assert.Equal(0, command.Attempts);
    }
}
