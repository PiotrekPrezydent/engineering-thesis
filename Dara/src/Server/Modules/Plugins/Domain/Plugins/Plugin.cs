using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;
using Dara.Server.Modules.Plugins.Domain.Plugins.Events;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

// one plugin .dll of one owner; it never changes, a new .dll is a new plugin
public class Plugin : Entity, IAggregateRoot
{
    public PluginId Id { get; private set; }
    
    public PluginOwnerId OwnerId { get; private set; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public string Version { get; private set; }
    
    public PluginHash Hash { get; private set; }
    
    public IReadOnlyList<PluginFunction> Functions => _functions.AsReadOnly();
    private List<PluginFunction> _functions;
    
    private Plugin() { }

    private Plugin(PluginOwnerId ownerId, string name, string? description, string version, PluginHash hash, IReadOnlyList<FunctionDescription> functions)
    {
        Id = new PluginId(Guid.NewGuid());
        
        OwnerId = ownerId;
        Name = name;
        Description = description;
        Version = version;
        Hash = hash;
        
        _functions = functions.Select(f => PluginFunction.Create(Id, f)).ToList();
        
        AddDomainEvent(new PluginAddedDomainEvent(OwnerId, Id, _functions.Select(f => f.Id).ToList()));
    }

    internal static Plugin Create(PluginOwnerId ownerId, string name, string? description, string version, PluginHash hash, IReadOnlyList<FunctionDescription> functions)
    {
        return new Plugin(ownerId, name, description, version, hash, functions);
    }

    internal void Remove()
    {
        AddDomainEvent(new PluginRemovedDomainEvent(OwnerId, Id));
    }
}