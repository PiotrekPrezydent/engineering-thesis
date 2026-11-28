using Dara.Server.Apps.API.Common.Authentication;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

// a client token only (claims sub and client); an account cookie gets 403 at the negotiation. The hub only sends:
// notifications and calls of the server; every command and query of a client goes over http (TASK-ENG-032-D01)
[Authorize(Policy = AuthPolicies.Client)]
public class AppHub : Hub<IAppHubClient>
{
    private readonly ClientConnections _connections;
    private readonly ILogger<AppHub> _logger;

    public AppHub(ClientConnections connections, ILogger<AppHub> logger)
    {
        _connections = connections;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _connections.Add(Context.ClientId(), Context);
        _logger.LogInformation("{ConnectionId} connected, client {ClientId}", Context.ConnectionId, Context.ClientId());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.Remove(Context.ClientId(), Context.ConnectionId);
        _logger.LogInformation("{ConnectionId} disconnected, client {ClientId}", Context.ConnectionId, Context.ClientId());
        await base.OnDisconnectedAsync(exception);
    }
}
