using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Identity.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Identity.Notifications;

public class NotifyClientRenamedHandler : IHubNotificationHandler<UserClientRenamedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyClientRenamedHandler(IHubContext<AppHub, IAppHubClient> context)
    {
        _context = context;
    }

    // only the connections of the client; members of its groups are told in TASK-ENG-030 and 032
    public async Task HandleAsync(UserClientRenamedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.ClientId.ToString());

        await client.OnClientRenamed(new ClientRenamedNotification(notification.ClientId, notification.NewName));
    }
}
