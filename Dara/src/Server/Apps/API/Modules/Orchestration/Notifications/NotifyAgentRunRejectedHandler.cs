using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Orchestration.Notifications;

public class NotifyAgentRunRejectedHandler : IHubNotificationHandler<AgentRunRejectedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyAgentRunRejectedHandler(IHubContext<AppHub, IAppHubClient> context)
    {
        _context = context;
    }

    public async Task HandleAsync(AgentRunRejectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.Group(HubGroups.Client(notification.AuthorId));

        await client.OnAgentRunRejected(new AgentRunRejectedNotification(notification.AgentRunId, notification.GroupId));
    }
}