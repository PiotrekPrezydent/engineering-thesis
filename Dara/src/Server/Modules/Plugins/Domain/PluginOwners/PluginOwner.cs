using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public class PluginOwner : Entity, IAggregateRoot
{
    public PluginOwnerId Id { get; private set; }

    private PluginOwner() { }

    private PluginOwner(PluginOwnerId id)
    {
        Id = id;
    }

    public static PluginOwner CreateDefault(PluginOwnerId pluginOwnerId)
    {
        return new PluginOwner(pluginOwnerId);
    }
    
    // owned plugins are the plugins of this owner now; the hash of the .dll identifies a plugin
    public Plugin AddPlugin(IReadOnlyList<Plugin> ownedPlugins, string name, string? description, string version,
        PluginHash hash, IReadOnlyList<FunctionDescription> functions)
    {
        CheckRule(new PluginHashMustBeSha256Rule(hash));
        CheckRule(new PluginNameMustBeProvidedRule(name));
        CheckRule(new TypeDescriptionMustBeValidRule(functions));
        CheckRule(new FunctionNamesMustBeUniqueRule(functions));
        CheckRule(new PropertyNamesMustBeUniqueRule(functions));
        CheckRule(new NamesMustFitProviderLimitsRule(functions));

        // the same .dll always gets the same key
        var lowercaseHash = new PluginHash(hash.Value.ToLowerInvariant());
        var current = OwnPlugins(ownedPlugins);

        CheckRule(new PluginCannotBeAddedTwiceRule(current, lowercaseHash));
        CheckRule(new PluginNameMustBeUniqueForOwnerRule(current, name));

        return Plugin.Create(Id, name, description, version, lowercaseHash, functions);
    }

    public void RemovePlugin(IReadOnlyList<Plugin> ownedPlugins, Plugin plugin)
    {
        CheckRule(new NonActualPluginCannotBeRemovedRule(OwnPlugins(ownedPlugins), plugin.Id));
        
        plugin.Remove();
    }

    private List<Plugin> OwnPlugins(IReadOnlyList<Plugin> plugins)
    {
        return plugins.Where(p => p.OwnerId == Id).ToList();
    }
}