namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging;

// delay after the n-th failure = min(BaseDelay * 2^(n-1), MaxDelay); no limit of attempts, a message is never dropped
public sealed record RetryPolicy
{
    public TimeSpan BaseDelay { get; }
    public TimeSpan MaxDelay { get; }

    public RetryPolicy(TimeSpan baseDelay, TimeSpan maxDelay)
    {
        if (baseDelay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(baseDelay), "The base delay cannot be negative.");
        if (maxDelay < baseDelay)
            throw new ArgumentOutOfRangeException(nameof(maxDelay), "The maximum delay cannot be shorter than the base delay.");

        BaseDelay = baseDelay;
        MaxDelay = maxDelay;
    }

    public TimeSpan GetDelay(int failedAttempts)
    {
        if (failedAttempts < 1)
            return TimeSpan.Zero;

        // 2^62 ticks would overflow long, and any delay of 2^30 base delays is far above a sensible maximum
        var exponent = Math.Min(failedAttempts - 1, 30);
        var ticks = BaseDelay.Ticks * Math.Pow(2, exponent);

        return ticks >= MaxDelay.Ticks ? MaxDelay : TimeSpan.FromTicks((long)ticks);
    }
}
