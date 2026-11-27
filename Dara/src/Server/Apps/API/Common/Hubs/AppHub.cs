using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Plugins.Application;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

// a client token only (claims sub and client); an account cookie gets 403 at the negotiation; the methods of each
// module are a partial file in the folder of the module (Modules/<Module>/AppHub.<Module>.cs) with this namespace
[Authorize(Policy = AuthPolicies.Client)]
public partial class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IGroupsModule _groupsModule;
    private readonly IPluginsModule _pluginsModule;
    private readonly IOrchestrationModule _orchestrationModule;
    private readonly ClientConnections _connections;
    private readonly ILogger<AppHub> _logger;

    public AppHub(IGroupsModule groupsModule, IPluginsModule pluginsModule, IOrchestrationModule orchestrationModule, ClientConnections connections, ILogger<AppHub> logger)
    {
        _groupsModule = groupsModule;
        _pluginsModule = pluginsModule;
        _orchestrationModule = orchestrationModule;
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
