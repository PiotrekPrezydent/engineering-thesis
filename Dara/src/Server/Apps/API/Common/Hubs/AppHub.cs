using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Presence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

// a client token only (claims sub and client); an account cookie gets 403 at the negotiation. The hub only sends:
// notifications and calls of the server; every command and query of a client goes over http (TASK-ENG-032-D01).
// Each connection joins the groups of its client and its account; the first connection and the last disconnection of
// a client are published as ClientConnected and ClientDisconnected for the modules
[Authorize(Policy = AuthPolicies.Client)]
public class AppHub : Hub<IAppHubClient>
{
    private readonly ClientPresenceTracker _presence;
    private readonly IEventBus _bus;
    private readonly ILogger<AppHub> _logger;

    public AppHub(ClientPresenceTracker presence, IEventBus bus, ILogger<AppHub> logger)
    {
        _presence = presence;
        _bus = bus;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var clientId = Context.ClientId();
        var userId = Context.UserId();
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["ClientId"] = clientId, ["UserId"] = userId });

        // the groups first, so a call sent right after IsOnline reaches this connection
        await Groups.AddToGroupAsync(Context.ConnectionId, HubGroups.Client(clientId));
        await Groups.AddToGroupAsync(Context.ConnectionId, HubGroups.User(userId));

        var change = _presence.Connect(clientId, Context);
        _logger.LogInformation("{ConnectionId} connected, client {ClientId}", Context.ConnectionId, clientId);
        if (change.Changed)
            await _bus.PublishAsync(new ClientConnectedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, clientId, userId, change.Sequence));

        await base.OnConnectedAsync();
    }

    // signalr removes the connection from its groups by itself
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var clientId = Context.ClientId();
        var userId = Context.UserId();
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["ClientId"] = clientId, ["UserId"] = userId });

        var change = _presence.Disconnect(clientId, Context.ConnectionId);
        _logger.LogInformation("{ConnectionId} disconnected, client {ClientId}: {Reason}", Context.ConnectionId, clientId, exception?.Message ?? "closed");
        if (change.Changed)
            await _bus.PublishAsync(new ClientDisconnectedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, clientId, userId, change.Sequence));

        await base.OnDisconnectedAsync(exception);
    }
}
