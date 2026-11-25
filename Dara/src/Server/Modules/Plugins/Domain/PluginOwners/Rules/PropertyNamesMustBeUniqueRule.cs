using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

// parameters of a function and properties of every object are keys of one json object
public class PropertyNamesMustBeUniqueRule : IBuisnessRule
{
    private readonly IReadOnlyList<FunctionDescription> _functions;

    public PropertyNamesMustBeUniqueRule(IReadOnlyList<FunctionDescription> functions)
    {
        _functions = functions;
    }

    public string Message => "Parameter and property names must be unique within a function and an object";
    public bool IsBroken()
    {
        foreach (var function in _functions)
        {
            if (HasDuplicates(function.Parameters))
                return true;

            var types = function.Parameters.Select(p => p.Type)
                .Concat(new[] { function.Returns }.OfType<TypeDescription>())
                .SelectMany(Flatten);

            if (types.Any(type => type.Properties is not null && HasDuplicates(type.Properties)))
                return true;
        }

        return false;
    }

    private static bool HasDuplicates(IReadOnlyList<PropertyDescription> properties)
    {
        return properties.Select(p => p.Name).Distinct(StringComparer.Ordinal).Count() != properties.Count;
    }

    // the type and every type below it
    private static IEnumerable<TypeDescription> Flatten(TypeDescription type)
    {
        var children = (type.Properties ?? []).Select(p => p.Type)
            .Concat(new[] { type.Items, type.AdditionalProperties }.OfType<TypeDescription>());

        return children.SelectMany(Flatten).Prepend(type);
    }
}
