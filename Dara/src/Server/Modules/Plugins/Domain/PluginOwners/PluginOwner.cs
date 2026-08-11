using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public class PluginOwner : Entity, IAggregateRoot
{
    public PluginOwnerId Id { get; private set; }

    public IReadOnlyList<Plugin> Plugins => _plugins;
    
    private List<Plugin> _plugins;

    private PluginOwner() { }

    private PluginOwner(PluginOwnerId id)
    {
        Id = id;
        _plugins = new();
    }

    public static PluginOwner Create(PluginOwnerId pluginOwnerId)
    {
        return new PluginOwner(pluginOwnerId);
    }
    
    public Plugin BuildPlugin(IEnumerable<PluginFunction> pluginFunctions)
    {
        return Plugin.Create(this, [..pluginFunctions]);
    }

    public void RegisterPlugin(Plugin plugin)
    {
        CheckRule(new PluginCannotBeAddedTwiceRule(plugin, Plugins));
        
        _plugins.Add(plugin);
    }

    public void UnregisterPlugin(Plugin plugin)
    {
        CheckRule(new NonActualPluginCannotBeRemovedRule(plugin, Plugins));
        
        _plugins.Remove(plugin);
    }
}