using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Identity.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Identity.Notifications;

// from now on the access token of the client is refused; its open connections are told and then closed, so the
// client signs out at once instead of when its token expires
public class NotifyClientRevokedHandler : IHubNotificationHandler<UserClientRevokedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;
    private readonly RevokedClientsRegistry _revokedClients;
    private readonly ClientConnections _connections;

    public NotifyClientRevokedHandler(IHubContext<AppHub, IAppHubClient> context, RevokedClientsRegistry revokedClients, ClientConnections connections)
    {
        _context = context;
        _revokedClients = revokedClients;
        _connections = connections;
    }

    public async Task HandleAsync(UserClientRevokedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _revokedClients.Add(notification.ClientId);

        var client = _context.Clients.User(notification.ClientId.ToString());
        await client.OnClientRevoked(new ClientRevokedNotification(notification.ClientId));

        _connections.CloseAll(notification.ClientId);
    }
}
