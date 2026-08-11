using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public record PluginFunction : IValueObject
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string ReturnType { get; init; }
    public ImmutableArray<PluginFunctionParameter> Parameters { get; init; }

    private PluginFunction() { }

    public PluginFunction(string Name, string Description, string ReturnType, ImmutableArray<PluginFunctionParameter> Parameters)
    {
        this.Name = Name;
        this.Description = Description;
        this.ReturnType = ReturnType;
        this.Parameters = Parameters;
    }
}