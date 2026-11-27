using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Server.Apps.API.Modules.Plugins;

// dtos of the plugins module in the shape of the client contract
public static class PluginContractMapping
{
    public static PluginSnapshot ToSnapshot(this OwnerPluginDto plugin)
    {
        var functions = plugin.Functions
            .Select(f => new PluginFunctionSnapshot(f.FunctionId, f.IsEnabled, f.Description))
            .ToList();

        return new PluginSnapshot(plugin.PluginId, plugin.Name, plugin.Description, plugin.Version, plugin.Sha256, plugin.IsEnabled, functions);
    }
}
