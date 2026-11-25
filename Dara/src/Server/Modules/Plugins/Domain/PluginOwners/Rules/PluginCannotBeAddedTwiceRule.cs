using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class PluginCannotBeAddedTwiceRule : IBuisnessRule
{
    private readonly IReadOnlyList<Plugin> _currentPlugins;
    private readonly PluginHash _hash;


    public PluginCannotBeAddedTwiceRule(IReadOnlyList<Plugin> currentPlugins, PluginHash hash)
    {
        _currentPlugins = currentPlugins;
        _hash = hash;
    }

    public string Message => "Plugin with this hash is already added";
    public bool IsBroken()
    {
        return _currentPlugins.Any(e => e.Hash == _hash);
    }
}