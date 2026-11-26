using Dara.Server.BuildingBlocks.Infrastructure.Messaging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration;

// how the outbox, inbox and internal commands workers of one module process and retry messages;
// the API host keeps the defaults, test hosts set short delays
public sealed record MessageProcessingOptions
{
    // messages of the outbox and the inbox taken in one pass
    public int BatchSize { get; init; } = 20;

    // the longest sleep of a worker without a signal
    public TimeSpan IdleTimeout { get; init; } = TimeSpan.FromSeconds(30);

    public RetryPolicy Retry { get; init; } = new(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));

    // internal commands of the module executed at once
    public int InternalCommandsParallelism { get; init; } = 4;

    // pending internal commands the worker looks at in one pass, the oldest first
    public int InternalCommandsLookahead { get; init; } = 200;

    // limits of concurrency groups (IConcurrencyLimitedInternalCommand); a group without an entry has no own limit
    public IReadOnlyDictionary<string, int> ConcurrencyLimits { get; init; } = new Dictionary<string, int>();
}
