using Dara.Server.Apps.API.Modules.Plugins;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Contracts.StateSnapshots;

// the namespace of the hub, not of this folder: a partial class lives in one namespace
namespace Dara.Server.Apps.API.Common.Hubs;

public partial class AppHub
{
    public async Task RegisterPlugins(IReadOnlyList<PluginManifest> plugins)
    {
        await _pluginsModule.ExecuteCommandAsync(new RegisterPluginsCommand(Context.ClientId(), plugins));
    }

    public async Task<IReadOnlyList<PluginSnapshot>> GetMyPlugins()
    {
        var plugins = await _pluginsModule.ExecuteQueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(Context.ClientId()));
        return plugins.Select(p => p.ToSnapshot()).ToList();
    }

    public async Task EnablePlugin(Guid pluginId)
    {
        await _pluginsModule.ExecuteCommandAsync(new EnablePluginCommand(Context.ClientId(), pluginId));
    }

    public async Task DisablePlugin(Guid pluginId)
    {
        await _pluginsModule.ExecuteCommandAsync(new DisablePluginCommand(Context.ClientId(), pluginId));
    }

    public async Task EnablePluginFunction(Guid pluginId, Guid functionId)
    {
        await _pluginsModule.ExecuteCommandAsync(new EnablePluginFunctionCommand(Context.ClientId(), pluginId, functionId));
    }

    public async Task DisablePluginFunction(Guid pluginId, Guid functionId)
    {
        await _pluginsModule.ExecuteCommandAsync(new DisablePluginFunctionCommand(Context.ClientId(), pluginId, functionId));
    }
}
