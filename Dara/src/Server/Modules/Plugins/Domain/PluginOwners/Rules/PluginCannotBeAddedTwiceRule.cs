using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class PluginCannotBeAddedTwiceRule : IBuisnessRule
{
    private readonly IReadOnlyList<Plugin> _currentPlugins;
    private readonly Plugin _pluginToAdd;


    public PluginCannotBeAddedTwiceRule(IReadOnlyList<Plugin> currentPlugins, Plugin pluginToAdd)
    {
        _currentPlugins = currentPlugins;
        _pluginToAdd = pluginToAdd;
    }

    public string Message => nameof(PluginCannotBeAddedTwiceRule);
    public bool IsBroken()
    {
        return _currentPlugins.Any(e=>e.Id == _pluginToAdd.Id);
    }
}