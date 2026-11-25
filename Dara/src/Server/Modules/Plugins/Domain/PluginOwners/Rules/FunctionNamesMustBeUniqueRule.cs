using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

// names differing only in letter case would collide in tool aliases
public class FunctionNamesMustBeUniqueRule : IBuisnessRule
{
    private readonly IReadOnlyList<FunctionDescription> _functions;

    public FunctionNamesMustBeUniqueRule(IReadOnlyList<FunctionDescription> functions)
    {
        _functions = functions;
    }

    public string Message => "Function names must be unique within a plugin";
    public bool IsBroken()
    {
        return _functions
            .GroupBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .Any(g => g.Count() > 1);
    }
}
