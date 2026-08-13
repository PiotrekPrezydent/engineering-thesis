using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Rules;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public class PluginFunction : Entity
{
    public PluginFunctionId Id { get; private set; }
    
    public Plugin Plugin { get; private set; }
    public PluginId PluginId { get; private set; }
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ReturnType { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public IReadOnlyList<PluginFunctionParameter> Parameters { get; init; }

    private PluginFunction() { }

    private PluginFunction(string name, string description, string returnType, IEnumerable<PluginFunctionParameter> parameters)
    {
        Id = new PluginFunctionId(Guid.NewGuid());
        
        Name = name;
        Description = description;
        ReturnType = returnType;
        IsActive = true;
        
        Parameters = parameters.ToList();
    }

    public static PluginFunction Create(string name, string description, string returnType,
        IEnumerable<PluginFunctionParameter> parameters)
    {
        return new PluginFunction(name, description, returnType, parameters);
    }

    internal void Activate()
    {
        CheckRule(new OnlyNonActiveFunctionCanBeActivated(IsActive));
        
        IsActive = true;
        AddDomainEvent(new PluginFunctionActivatedDomainEvent(Plugin.OwnerId, Plugin.Id, Id));
    }
    internal void Deactivate()
    {
        CheckRule(new OnlyActiveFunctionCanBeDeactivated(IsActive));
        
        IsActive = false;
        AddDomainEvent(new PluginFunctionDeactivatedDomainEvent(Plugin.OwnerId, Plugin.Id, Id));
    }
}