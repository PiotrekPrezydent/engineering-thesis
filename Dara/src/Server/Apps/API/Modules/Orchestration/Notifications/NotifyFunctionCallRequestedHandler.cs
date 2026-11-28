using Dara.Server.Apps.API.Common.FunctionCalls;
using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Apps.API.Common.Presence;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.FunctionCalls;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Orchestration.Notifications;

// a call of orchestration goes to the connections of its client without waiting for the result, which comes over
// http; an offline client gets nothing and the port hears NotDelivered at once. A disconnection between the check and
// the send is caught by the time limit of the step in orchestration (TASK-ENG-013)
public class NotifyFunctionCallRequestedHandler : IHubNotificationHandler<FunctionCallRequestedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;
    private readonly IClientPresence _presence;
    private readonly PendingFunctionCalls _pending;
    private readonly IFunctionCallResultPort _port;
    private readonly TimeProvider _time;
    private readonly ILogger<NotifyFunctionCallRequestedHandler> _logger;

    public NotifyFunctionCallRequestedHandler(IHubContext<AppHub, IAppHubClient> context, IClientPresence presence, PendingFunctionCalls pending,
        IFunctionCallResultPort port, TimeProvider time, ILogger<NotifyFunctionCallRequestedHandler> logger)
    {
        _context = context;
        _presence = presence;
        _pending = pending;
        _port = port;
        _time = time;
        _logger = logger;
    }

    public async Task HandleAsync(FunctionCallRequestedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["ClientId"] = notification.TargetClientId, ["AgentRunId"] = notification.AgentRunId });

        if (!_presence.IsOnline(notification.TargetClientId))
        {
            _logger.LogInformation("call {CallId} not delivered, client {ClientId} is offline", notification.CallId, notification.TargetClientId);
            await _port.AcceptAsync(new FunctionCallResult(notification.CallId, notification.TargetClientId, FunctionCallResultOutcome.NotDelivered, null,
                "the client is offline", _time.GetUtcNow().UtcDateTime), cancellationToken);
            return;
        }

        _pending.Register(notification.CallId, notification.TargetClientId, notification.ExpiresAt);
        await _context.Clients.Group(HubGroups.Client(notification.TargetClientId)).CallFunction(new FunctionCallRequest(
            notification.CallId, notification.PluginName, notification.PluginHash, notification.FunctionName, notification.ArgumentsJson));
    }
}
