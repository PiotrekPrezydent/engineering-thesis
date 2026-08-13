using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public class PluginOwner : Entity, IAggregateRoot
{
    public PluginOwnerId Id { get; private set; }

    public IReadOnlyList<Plugin> Plugins => _plugins.AsReadOnly();

    private List<Plugin> _plugins;

    private PluginOwner() { }

    private PluginOwner(PluginOwnerId id, List<Plugin> plugins)
    {
        Id = id;
        
        _plugins = plugins;
    }

    public static PluginOwner CreateDefault(PluginOwnerId pluginOwnerId)
    {
        return new PluginOwner(pluginOwnerId, new());
    }
    
    public void AddPlugin(string name, string description, List<PluginFunction> functions)
    {
        var plugin = Plugin.Create(name, description, functions);
        
        CheckRule(new PluginCannotBeAddedTwiceRule(Plugins, plugin));
        _plugins.Add(plugin);
        
        AddDomainEvent(new PluginAddedDomainEvent(Id, plugin.Id));
    }

    public void RemovePlugin(PluginId pluginId)
    {
        CheckRule(new NonActualPluginCannotBeRemovedRule(Plugins, pluginId));
        
        var plugin = _plugins.Single(e=>e.Id == pluginId);
        _plugins.Remove(plugin);
        
        AddDomainEvent(new PluginRemovedDomainEvent(Id, plugin.Id));
    }

    public void ActivateFunction(PluginId pluginId, PluginFunctionId functionId)
    {
        CheckRule(new OnlyActualPluginCanBeConfigured(_plugins, pluginId));
        
        var plugin = _plugins.Single(e=>e.Id == pluginId);
        CheckRule(new OnlyActualPluginFunctionCanBeConfigured(plugin.Functions, functionId));
        
        var function = plugin.Functions.Single(e=>e.Id == functionId);
        function.Activate();
    }

    public void DeactivateFunction(PluginId pluginId, PluginFunctionId functionId)
    {
        CheckRule(new OnlyActualPluginCanBeConfigured(_plugins, pluginId));
        
        var plugin = _plugins.Single(e=>e.Id == pluginId);
        CheckRule(new OnlyActualPluginFunctionCanBeConfigured(plugin.Functions, functionId));
        
        var function = plugin.Functions.Single(e=>e.Id == functionId);
        function.Deactivate();
    }
}