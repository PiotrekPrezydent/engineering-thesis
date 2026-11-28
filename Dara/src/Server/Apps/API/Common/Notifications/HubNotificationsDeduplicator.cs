using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Apps.API.Common.Notifications;

// a retried outbox message publishes its integration event again with the same EventId (another subscriber of the bus
// failed); the hub has no inbox, so it remembers the ids it forwarded and lets every event through only once.
// the key is the type with the id: handlers of one domain notification may publish several events with its id.
// in memory only: a restart of the host forgets them, and an event older than the last Capacity events would be
// forwarded again; the retry delays of the outbox (at most minutes) stay far inside that window
public sealed class HubNotificationsDeduplicator
{
    public const int DefaultCapacity = 10_000;

    private readonly Lock _lock = new();
    private readonly HashSet<(Type, Guid)> _seen = new();
    private readonly Queue<(Type, Guid)> _order = new();

    public int Capacity { get; }

    public HubNotificationsDeduplicator(int capacity = DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        Capacity = capacity;
    }

    // true the first time the event comes, false for every repeat; the oldest id is forgotten when the set is full
    public bool TryAccept(IIntegrationEvent integrationEvent)
    {
        var key = (integrationEvent.GetType(), integrationEvent.EventId);

        lock (_lock)
        {
            if (!_seen.Add(key))
                return false;

            _order.Enqueue(key);
            if (_order.Count > Capacity)
                _seen.Remove(_order.Dequeue());

            return true;
        }
    }
}
