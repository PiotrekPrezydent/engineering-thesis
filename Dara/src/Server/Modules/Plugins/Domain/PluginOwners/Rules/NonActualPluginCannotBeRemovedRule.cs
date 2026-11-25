using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class NonActualPluginCannotBeRemovedRule : IBuisnessRule
{
    private readonly IReadOnlyList<Plugin> _currentPlugins;
    private readonly PluginId _pluginToRemove;
    
    public NonActualPluginCannotBeRemovedRule(IReadOnlyList<Plugin> currentPlugins, PluginId pluginToRemove)
    {
        _pluginToRemove = pluginToRemove;
        _currentPlugins = currentPlugins;
    }

    public string Message => "Only a current plugin of the owner can be removed";
    public bool IsBroken()
    {
        return _currentPlugins.All(e => e.Id != _pluginToRemove);
    }
}