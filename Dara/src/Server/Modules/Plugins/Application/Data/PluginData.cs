using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.Data;

public record PluginData(string Name, string Description)
{
    internal static PluginData FromPlugin(Plugin plugin) => new(plugin.Name, plugin.Description);
}
