namespace Dara.Server.Apps.API.Common.FunctionCalls;

public enum CompletionResult
{
    Accepted,
    // no such call or after ExpiresAt
    Unknown,
    // the call went to another client; answered like Unknown, so a client learns nothing about calls of others
    WrongClient,
    AlreadyCompleted
}

// the calls sent to clients and waiting for a result, in memory (one server): which client may answer and until when.
// A taken result stays until ExpiresAt, so a repeat gets 409 and not 404. The time limit of a step is orchestration's
// (TASK-ENG-013); the host only forgets an entry after ExpiresAt
public sealed class PendingFunctionCalls
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, Entry> _calls = new();
    private readonly TimeProvider _time;

    public PendingFunctionCalls(TimeProvider time)
    {
        _time = time;
    }

    public void Register(Guid callId, Guid clientId, DateTime expiresAt)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        lock (_lock)
        {
            foreach (var expired in _calls.Where(c => c.Value.ExpiresAt <= now).Select(c => c.Key).ToList())
                _calls.Remove(expired);

            _calls[callId] = new Entry(clientId, expiresAt, false);
        }
    }

    public CompletionResult TryComplete(Guid callId, Guid callerClientId)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        lock (_lock)
        {
            if (!_calls.TryGetValue(callId, out var entry) || entry.ExpiresAt <= now)
                return CompletionResult.Unknown;
            if (entry.ClientId != callerClientId)
                return CompletionResult.WrongClient;
            if (entry.Completed)
                return CompletionResult.AlreadyCompleted;

            _calls[callId] = entry with { Completed = true };
            return CompletionResult.Accepted;
        }
    }

    private readonly record struct Entry(Guid ClientId, DateTime ExpiresAt, bool Completed);
}
