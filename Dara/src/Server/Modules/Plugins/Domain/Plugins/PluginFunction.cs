using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public class PluginFunction : Entity
{
    public PluginFunctionId Id { get; private set; }
    
    public Plugin Plugin { get; private set; }
    public PluginId PluginId { get; private set; }
    
    public string Name { get; private set; }
    public string? Description { get; private set; }
    
    public IReadOnlyList<PropertyDescription> Parameters { get; private set; }
    
    // null represent void function
    public TypeDescription? Returns { get; private set; }

    private PluginFunction() { }

    private PluginFunction(PluginId pluginId, FunctionDescription description)
    {
        Id = new PluginFunctionId(Guid.NewGuid());
        
        PluginId = pluginId;
        Name = description.Name;
        Description = description.Description;
        Parameters = description.Parameters;
        Returns = description.Returns;
    }
        
    internal static PluginFunction Create(PluginId pluginId, FunctionDescription description)
    {
        return new PluginFunction(pluginId, description);
    }

    public FunctionDescription GetDescription()
    {
        return new FunctionDescription(Name, Description, Parameters, Returns);
    }
}