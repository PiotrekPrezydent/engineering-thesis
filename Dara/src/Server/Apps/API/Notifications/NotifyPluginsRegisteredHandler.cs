using Dara.Server.Apps.API.Extensions;
using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

// every change of one plugin sends its owner the whole registered set
public class NotifyPluginsRegisteredHandler : IHubNotificationHandler<PluginFunctionsChangedIntegrationEvent>
{
    private readonly IPluginsModule _pluginsModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyPluginsRegisteredHandler(IPluginsModule pluginsModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _pluginsModule = pluginsModule;
        _context = context;
    }

    public async Task HandleAsync(PluginFunctionsChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.PluginOwnerId.ToString());
        var plugins = await _pluginsModule.ExecuteQueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(notification.PluginOwnerId));

        await client.OnPluginsRegistered(new PluginsRegisteredNotification(plugins.Select(p => p.ToSnapshot()).ToList()));
    }
}