using Dara.Server.BuildingBlocks.Domain;

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
    
}