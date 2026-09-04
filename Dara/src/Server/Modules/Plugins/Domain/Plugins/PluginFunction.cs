using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public class PluginFunction : Entity
{
    public PluginFunctionId Id { get; private set; }
    
    public Plugin Plugin { get; private set; }
    public PluginId PluginId { get; private set; }
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    // null represent void function
    public TypeData? ReturnTypeData { get; private set; }
    
    public IReadOnlyList<FunctionParameterData> Parameters { get; private set; }

    private PluginFunction() { }

    private PluginFunction(string name, string description, TypeData? returnTypeData, IEnumerable<FunctionParameterData> parameters)
    {
        Id = new PluginFunctionId(Guid.NewGuid());
        
        Name = name;
        Description = description;
        ReturnTypeData = returnTypeData;
        
        Parameters = parameters.ToList();
    }

    public static PluginFunction Create(string name, string description, TypeData? returnTypeData, IEnumerable<FunctionParameterData> parameters)
    {
        return new PluginFunction(name, description, returnTypeData, parameters);
    }
}