using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public class PluginOwner : Entity, IAggregateRoot
{
    public PluginOwnerId ClientId { get; set; }
    
    public List<Plugin> Plugins { get; set; }

    private PluginOwner() { }

    public PluginOwner(PluginOwnerId clientId)
    {
        ClientId = clientId;
        Plugins = new();
    }
}