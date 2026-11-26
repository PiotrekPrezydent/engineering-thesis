using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

namespace Dara.Server.BuildingBlocks.UnitTests;

public class InternalCommandSelectorTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Dictionary<string, int> NoLimits = new();

    private static PendingInternalCommand Ready(string? key = null, string? group = null) => new(Guid.NewGuid(), Now, key, group);

    private static PendingInternalCommand Waiting(TimeSpan inFuture, string? key = null) => new(Guid.NewGuid(), Now + inFuture, key, null);

    private static RunningInternalCommand Running(PendingInternalCommand command) => new(command.Id, command.OrderingKey, command.ConcurrencyGroup);

    [Fact]
    public void ReadyCommandsWithoutKeysRunUpToTheFreeSlots()
    {
        var pending = new[] { Ready(), Ready(), Ready() };

        var selection = InternalCommandSelector.Select(pending, [], Now, 2, NoLimits);

        Assert.Equal(pending.Take(2), selection.Ready);
    }

    [Fact]
    public void LaterCommandOfAKeyWaitsForTheOlderOne()
    {
        var first = Ready("group:1");
        var second = Ready("group:1");
        var other = Ready("group:2");

        var selection = InternalCommandSelector.Select([first, second, other], [], Now, 4, NoLimits);

        Assert.Equal([first, other], selection.Ready);
    }

    [Fact]
    public void OlderCommandWaitingForARetryHoldsBackItsKey()
    {
        var failed = Waiting(TimeSpan.FromSeconds(4), "group:1");
        var later = Ready("group:1");
        var free = Ready();

        var selection = InternalCommandSelector.Select([failed, later, free], [], Now, 4, NoLimits);

        Assert.Equal([free], selection.Ready);
        Assert.Equal(failed.NextAttemptAt, selection.NextAttemptAt);
    }

    [Fact]
    public void RunningCommandHoldsItsKeyAndIsNotStartedAgain()
    {
        var running = Ready("run:1");
        var next = Ready("run:1");

        var selection = InternalCommandSelector.Select([running, next], [Running(running)], Now, 3, NoLimits);

        Assert.Empty(selection.Ready);
    }

    [Fact]
    public void GroupRunsNoMoreThanItsLimit()
    {
        var running = Ready(group: "plan-generation");
        var second = Ready(group: "plan-generation");
        var third = Ready(group: "plan-generation");
        var other = Ready();
        var limits = new Dictionary<string, int> { ["plan-generation"] = 2 };

        var selection = InternalCommandSelector.Select([running, second, third, other], [Running(running)], Now, 3, limits);

        Assert.Equal([second, other], selection.Ready);
    }

    [Fact]
    public void GroupWithoutALimitUsesAllSlots()
    {
        var pending = new[] { Ready(group: "free"), Ready(group: "free"), Ready(group: "free") };

        var selection = InternalCommandSelector.Select(pending, [], Now, 3, NoLimits);

        Assert.Equal(pending, selection.Ready);
    }

    [Fact]
    public void NextAttemptIsTheEarliestRetry()
    {
        var late = Waiting(TimeSpan.FromMinutes(5));
        var soon = Waiting(TimeSpan.FromSeconds(1));

        var selection = InternalCommandSelector.Select([late, soon], [], Now, 4, NoLimits);

        Assert.Empty(selection.Ready);
        Assert.Equal(soon.NextAttemptAt, selection.NextAttemptAt);
    }
}
