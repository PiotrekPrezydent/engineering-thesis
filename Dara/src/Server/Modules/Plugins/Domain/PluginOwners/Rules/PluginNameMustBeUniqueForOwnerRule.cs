using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

// a new .dll of a plugin replaces the old one, so the old one is removed first
public class PluginNameMustBeUniqueForOwnerRule : IBuisnessRule
{
    private readonly IReadOnlyList<Plugin> _currentPlugins;
    private readonly string _name;

    public PluginNameMustBeUniqueForOwnerRule(IReadOnlyList<Plugin> currentPlugins, string name)
    {
        _currentPlugins = currentPlugins;
        _name = name;
    }

    public string Message => "Owner already has a plugin with this name";
    public bool IsBroken()
    {
        return _currentPlugins.Any(e => e.Name == _name);
    }
}
