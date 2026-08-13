using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class OnlyActualPluginCanBeConfigured : IBuisnessRule
{
    private readonly IReadOnlyList<Plugin> _currentPlugins;
    private readonly PluginId _pluginToConfigure;

    public OnlyActualPluginCanBeConfigured(IReadOnlyList<Plugin> currentPlugins, PluginId pluginToConfigure)
    {
        _currentPlugins = currentPlugins;
        _pluginToConfigure = pluginToConfigure;
    }

    public string Message => nameof(OnlyActualPluginCanBeConfigured);
    public bool IsBroken()
    {
        return _currentPlugins.All(e=>e.Id != _pluginToConfigure);
    }
}