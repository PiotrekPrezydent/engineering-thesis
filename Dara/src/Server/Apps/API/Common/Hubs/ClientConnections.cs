using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

// the open hub connections of every client, so a revocation over http can close them at once instead of waiting for
// the access token to expire; the identity of a connection is checked once, when it connects
public class ClientConnections
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, HubCallerContext>> _connections = new();

    public void Add(Guid clientId, HubCallerContext context)
    {
        _connections.GetOrAdd(clientId, _ => new()).TryAdd(context.ConnectionId, context);
    }

    public void Remove(Guid clientId, string connectionId)
    {
        if (_connections.TryGetValue(clientId, out var connections))
            connections.TryRemove(connectionId, out _);
    }

    public int Count(Guid clientId)
    {
        return _connections.TryGetValue(clientId, out var connections) ? connections.Count : 0;
    }

    // every connection of the client ends; OnDisconnectedAsync removes them from here
    public void CloseAll(Guid clientId)
    {
        if (!_connections.TryGetValue(clientId, out var connections))
            return;

        foreach (var context in connections.Values)
            context.Abort();
    }
}
