using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Presence;

// is a client connected to the hub now; presence is the state of its hub connections, not the lifetime of its token
public interface IClientPresence
{
    bool IsOnline(Guid clientId);

    IReadOnlyCollection<Guid> OnlineClients();
}

// a change of the presence of one client: Changed only for its first connection and its last disconnection; Sequence
// grows over the whole host, so a receiver drops an event older than the last one it saw for the client
public readonly record struct PresenceChange(bool Changed, bool IsOnline, long Sequence);

// the open hub connections of every client in memory (one server): a client may have two connections while it
// renews its token (TASK-ENG-032-D08), an account many clients. The hub context of every connection is kept, so a
// revocation over http closes them at once; the entry goes with the connection. After a restart the map is empty and
// every client connects again
public sealed class ClientPresenceTracker : IClientPresence
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, Dictionary<string, HubCallerContext>> _connections = new();
    private long _sequence;

    public PresenceChange Connect(Guid clientId, HubCallerContext context)
    {
        lock (_lock)
        {
            if (!_connections.TryGetValue(clientId, out var connections))
                _connections[clientId] = connections = new Dictionary<string, HubCallerContext>();

            connections[context.ConnectionId] = context;
            return connections.Count == 1 ? new PresenceChange(true, true, ++_sequence) : new PresenceChange(false, true, _sequence);
        }
    }

    public PresenceChange Disconnect(Guid clientId, string connectionId)
    {
        lock (_lock)
        {
            if (!_connections.TryGetValue(clientId, out var connections) || !connections.Remove(connectionId))
                return new PresenceChange(false, IsOnlineLocked(clientId), _sequence);

            if (connections.Count > 0)
                return new PresenceChange(false, true, _sequence);

            _connections.Remove(clientId);
            return new PresenceChange(true, false, ++_sequence);
        }
    }

    public bool IsOnline(Guid clientId)
    {
        lock (_lock)
            return IsOnlineLocked(clientId);
    }

    public IReadOnlyCollection<Guid> OnlineClients()
    {
        lock (_lock)
            return _connections.Keys.ToList();
    }

    public int ConnectionCount(Guid clientId)
    {
        lock (_lock)
            return _connections.TryGetValue(clientId, out var connections) ? connections.Count : 0;
    }

    // every connection of the client ends; OnDisconnectedAsync removes them from here
    public void CloseAll(Guid clientId)
    {
        List<HubCallerContext> contexts;
        lock (_lock)
            contexts = _connections.TryGetValue(clientId, out var connections) ? connections.Values.ToList() : [];

        foreach (var context in contexts)
            context.Abort();
    }

    private bool IsOnlineLocked(Guid clientId)
    {
        return _connections.ContainsKey(clientId);
    }
}
