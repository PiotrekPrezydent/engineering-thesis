using System.Text.RegularExpressions;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

// common part of the gemini, openai and anthropic limits for function and parameter names
public class NamesMustFitProviderLimitsRule : IBuisnessRule
{
    private static readonly Regex AllowedName = new("^[A-Za-z_][A-Za-z0-9_]{0,63}$");

    private readonly IReadOnlyList<FunctionDescription> _functions;

    public NamesMustFitProviderLimitsRule(IReadOnlyList<FunctionDescription> functions)
    {
        _functions = functions;
    }

    public string Message => "Names must start with a letter or _, have only letters, digits and _ and at most 64 characters";
    public bool IsBroken()
    {
        return Names().Any(name => name is null || !AllowedName.IsMatch(name));
    }

    private IEnumerable<string?> Names()
    {
        foreach (var function in _functions)
        {
            yield return function.Name;

            var types = function.Parameters.Select(p => p.Type)
                .Concat(new[] { function.Returns }.OfType<TypeDescription>())
                .SelectMany(Flatten);

            var properties = function.Parameters
                .Concat(types.SelectMany(type => type.Properties ?? []));

            foreach (var property in properties)
                yield return property.Name;
        }
    }

    // the type and every type below it
    private static IEnumerable<TypeDescription> Flatten(TypeDescription type)
    {
        var children = (type.Properties ?? []).Select(p => p.Type)
            .Concat(new[] { type.Items, type.AdditionalProperties }.OfType<TypeDescription>());

        return children.SelectMany(Flatten).Prepend(type);
    }
}
