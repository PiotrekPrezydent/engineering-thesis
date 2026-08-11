using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class NonActualPluginCannotBeRemovedRule : IBuisnessRule
{
    private readonly Plugin _pluginToAdd;
    private readonly IReadOnlyList<Plugin> _currentPlugins;

    public NonActualPluginCannotBeRemovedRule(Plugin pluginToAdd, IReadOnlyList<Plugin> currentPlugins)
    {
        _pluginToAdd = pluginToAdd;
        _currentPlugins = currentPlugins;
    }

    public string Message => nameof(NonActualPluginCannotBeRemovedRule);
    public bool IsBroken()
    {
        return !_currentPlugins.Contains(_pluginToAdd);
    }
}