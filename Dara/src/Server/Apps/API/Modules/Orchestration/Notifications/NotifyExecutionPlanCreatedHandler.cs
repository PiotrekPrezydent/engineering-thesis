using Dara.Server.Apps.API.Modules.Orchestration;
using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Orchestration.Notifications;

// the plan goes only to its author, the only one who can approve or reject it
public class NotifyExecutionPlanCreatedHandler : IHubNotificationHandler<ExecutionPlanCreatedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyExecutionPlanCreatedHandler(IHubContext<AppHub, IAppHubClient> context)
    {
        _context = context;
    }

    public async Task HandleAsync(ExecutionPlanCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.AuthorId.ToString());
        var calls = notification.Calls.Select(c => c.ToSnapshot()).ToList();

        await client.OnExecutionPlanCreated(new ExecutionPlanCreatedNotification(notification.AgentRunId, notification.GroupId, calls));
    }
}