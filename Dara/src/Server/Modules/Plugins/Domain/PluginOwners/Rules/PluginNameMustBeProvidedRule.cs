using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class PluginNameMustBeProvidedRule : IBuisnessRule
{
    private readonly string? _name;

    public PluginNameMustBeProvidedRule(string? name)
    {
        _name = name;
    }

    public string Message => "Plugin name must be provided";
    public bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(_name);
    }
}
