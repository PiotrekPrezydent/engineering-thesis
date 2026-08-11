using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public class Plugin : Entity
{
    public PluginId Id { get; private set; }
    public PluginOwner Owner { get; private set; }
    
    public ImmutableArray<PluginFunction> Functions  { get; private set; }

    private Plugin(PluginOwner owner, ImmutableArray<PluginFunction> functions)
    {
        Id = new(Guid.NewGuid());
        Owner = owner;
        Functions = functions;
    }

    public static Plugin Create(PluginOwner owner, ImmutableArray<PluginFunction> functions)
    {
        return new Plugin(owner, functions);
    }
}