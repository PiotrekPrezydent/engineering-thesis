using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public record PluginFunctionParameter : IValueObject
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string Type { get; init; }

    private PluginFunctionParameter()
    {
    }

    public PluginFunctionParameter(string Name, string Description, string Type)
    {
        this.Name = Name;
        this.Description = Description;
        this.Type = Type;
    }
    
}