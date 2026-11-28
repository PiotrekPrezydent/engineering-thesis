using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Orchestration.Notifications;

public class NotifyAgentRunStartedHandler : IHubNotificationHandler<AgentRunStartedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyAgentRunStartedHandler(IHubContext<AppHub, IAppHubClient> context)
    {
        _context = context;
    }

    public async Task HandleAsync(AgentRunStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.Group(HubGroups.Client(notification.AuthorId));

        await client.OnAgentRunStarted(new AgentRunStartedNotification(notification.AgentRunId, notification.GroupId, notification.MessageId));
    }
}