using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

public class NotifyExecutionPlanApprovedHandler : IHubNotificationHandler<ExecutionPlanApprovedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyExecutionPlanApprovedHandler(IHubContext<AppHub, IAppHubClient> context)
    {
        _context = context;
    }

    public async Task HandleAsync(ExecutionPlanApprovedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.AuthorId.ToString());

        await client.OnExecutionPlanApproved(new ExecutionPlanApprovedNotification(notification.AgentRunId, notification.GroupId));
    }
}