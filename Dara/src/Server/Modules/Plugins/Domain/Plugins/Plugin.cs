using Dara.BuildingBlocks.Domain.Models;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public class Plugin : IAggregateRoot
{
    public PluginId Id { get; }
    public string Name { get; }

    public PluginOwnerId OwnerId { get; }

    public IReadOnlyList<PluginFunction> Functions { get; }
    
    private Plugin(PluginOwnerId ownerId, string name, IReadOnlyList<PluginFunction> functions)
    {
        Id = new PluginId(Guid.NewGuid());
        
        Name = name;
        Functions = functions;
        OwnerId = ownerId;
    }

    public static Plugin Create(PluginOwnerId ownerId, string name, IReadOnlyList<PluginFunction> functions)
    {
        return new Plugin(ownerId, name, functions);
    }
}