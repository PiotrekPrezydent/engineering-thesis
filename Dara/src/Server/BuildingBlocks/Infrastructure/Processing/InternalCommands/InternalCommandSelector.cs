namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public sealed record RunningInternalCommand(Guid Id, string? OrderingKey, string? ConcurrencyGroup);

public sealed record InternalCommandSelection(IReadOnlyList<PendingInternalCommand> Ready, DateTime? NextAttemptAt);

// picks the commands the worker starts now; pure, so the rules of order and concurrency are tested without a worker
public static class InternalCommandSelector
{
    // pending: unprocessed commands, the oldest first
    // a command is ready when it waits for no retry, is not running, no older unprocessed command with its ordering key
    // exists (FIFO per key, also when the older one waits for a retry) and its concurrency group is below its limit;
    // NextAttemptAt is the earliest retry in the future, so the worker knows how long it can sleep
    public static InternalCommandSelection Select(
        IReadOnlyList<PendingInternalCommand> pending,
        IReadOnlyCollection<RunningInternalCommand> running,
        DateTime now,
        int freeSlots,
        IReadOnlyDictionary<string, int> concurrencyLimits)
    {
        var runningIds = new HashSet<Guid>(running.Select(r => r.Id));
        var heldKeys = new HashSet<string>(running.Where(r => r.OrderingKey != null).Select(r => r.OrderingKey!));
        var groupCounts = running
            .Where(r => r.ConcurrencyGroup != null)
            .GroupBy(r => r.ConcurrencyGroup!)
            .ToDictionary(g => g.Key, g => g.Count());

        var ready = new List<PendingInternalCommand>();
        DateTime? nextAttemptAt = null;

        foreach (var command in pending)
        {
            if (runningIds.Contains(command.Id))
                continue;

            // the oldest unprocessed command of a key holds the key, the later ones wait for it
            if (command.OrderingKey != null && !heldKeys.Add(command.OrderingKey))
                continue;

            if (command.NextAttemptAt > now)
            {
                if (nextAttemptAt == null || command.NextAttemptAt < nextAttemptAt)
                    nextAttemptAt = command.NextAttemptAt;
                continue;
            }

            if (ready.Count >= freeSlots)
                continue;

            if (command.ConcurrencyGroup != null && concurrencyLimits.TryGetValue(command.ConcurrencyGroup, out var limit))
            {
                groupCounts.TryGetValue(command.ConcurrencyGroup, out var count);
                if (count >= limit)
                    continue;

                groupCounts[command.ConcurrencyGroup] = count + 1;
            }

            ready.Add(command);
        }

        return new InternalCommandSelection(ready, nextAttemptAt);
    }
}
