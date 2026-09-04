using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public class Plugin : Entity
{
    public PluginId Id { get; private set; }
    
    public PluginOwnerId OwnerId { get; private set; }
    
    public IReadOnlyList<PluginFunction> Functions => _functions.AsReadOnly();
    private List<PluginFunction> _functions;
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    private Plugin() { }

    private Plugin(PluginOwnerId ownerId, string name, string description, List<PluginFunction> functions)
    {
        Id = new(Guid.NewGuid());
        
        OwnerId = ownerId;
        Description = description;
        Name = name;
        
        _functions = functions;
    }

    public static Plugin Create(PluginOwnerId ownerId, string name, string description, List<PluginFunction> functions)
    {
        return new Plugin(ownerId, name,description, functions);
    }
}