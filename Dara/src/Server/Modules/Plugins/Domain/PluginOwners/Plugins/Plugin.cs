using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public class Plugin : Entity
{
    public PluginId Id { get; private set; }
    public PluginOwnerId OwnerId { get; private set; }
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public ImmutableArray<PluginFunction> Functions  { get; private set; }

    private Plugin() { }

    private Plugin(PluginOwnerId ownerId, string name, string description, ImmutableArray<PluginFunction> functions)
    {
        Id = new(Guid.NewGuid());
        OwnerId = ownerId;
        Functions = functions;
        Description = description;
        Name = name;
    }

    internal static Plugin Create(PluginOwnerId ownerId,string name, string description, ImmutableArray<PluginFunction> functions)
    {
        return new Plugin(ownerId,name,description, functions);
    }
}