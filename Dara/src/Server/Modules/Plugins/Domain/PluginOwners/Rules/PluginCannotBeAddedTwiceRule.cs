using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class PluginCannotBeAddedTwiceRule : IBuisnessRule
{
    private readonly Plugin _pluginToAdd;
    private readonly IReadOnlyList<Plugin> _currentPlugins;

    public PluginCannotBeAddedTwiceRule(Plugin pluginToAdd, IReadOnlyList<Plugin> currentPlugins)
    {
        _pluginToAdd = pluginToAdd;
        _currentPlugins = currentPlugins;
    }

    public string Message => nameof(PluginCannotBeAddedTwiceRule);
    public bool IsBroken()
    {
        return _currentPlugins.Contains(_pluginToAdd);
    }
}