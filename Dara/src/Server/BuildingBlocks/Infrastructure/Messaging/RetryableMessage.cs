namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging;

// state of a stored message that is processed exactly once and retried after a failure until it succeeds;
// a CLR base class only, every message type keeps its own table
public abstract class RetryableMessage
{
    public const int MaxErrorLength = 2000;

    // set in the same SaveChanges as the effect of the message
    public DateTime? ProcessedDate { get; protected set; }

    // number of failed attempts
    public int Attempts { get; protected set; }

    // exception.ToString() of the last failure, cut to MaxErrorLength
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? LastError { get; protected set; }

    // the earliest next attempt; the time of creation for a new message
    public DateTime NextAttemptAt { get; protected set; }

    protected RetryableMessage()
    {
    }

    protected RetryableMessage(DateTime createdAt)
    {
        NextAttemptAt = createdAt;
    }

    public void MarkProcessed(DateTime now)
    {
        ProcessedDate = now;
    }

    public void RecordFailure(Exception error, DateTime now, RetryPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(policy);

        Attempts++;

        var text = error.ToString();
        LastError = text.Length > MaxErrorLength ? text[..MaxErrorLength] : text;
        NextAttemptAt = now + policy.GetDelay(Attempts);
    }
}
