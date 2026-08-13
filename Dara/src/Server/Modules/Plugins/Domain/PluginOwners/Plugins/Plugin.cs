using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public class Plugin : Entity
{
    public PluginId Id { get; private set; }
    
    public PluginOwner Owner { get; private set; }
    public PluginOwnerId OwnerId { get; private set; }
    
    public IReadOnlyList<PluginFunction> Functions => _functions.AsReadOnly();
    private List<PluginFunction> _functions;
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    private Plugin() { }

    private Plugin(string name, string description, List<PluginFunction> functions)
    {
        Id = new(Guid.NewGuid());
        Description = description;
        Name = name;
        
        _functions = functions;
    }

    internal static Plugin Create(string name, string description, List<PluginFunction> functions)
    {
        return new Plugin(name,description, functions);
    }
}