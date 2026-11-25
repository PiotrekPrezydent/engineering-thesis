using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// names, required arguments and values against the function description, the message is for the model
public class CallArgumentsMustMatchFunctionRule : IBuisnessRule
{
    private static readonly TimeSpan PatternTimeout = TimeSpan.FromSeconds(1);

    private readonly FunctionDescription _function;
    private readonly IReadOnlyList<CallArgument> _arguments;
    private string? _problem;

    public CallArgumentsMustMatchFunctionRule(FunctionDescription function, IReadOnlyList<CallArgument> arguments)
    {
        _function = function;
        _arguments = arguments;
    }

    public string Message => $"Arguments of {_function.Name} are not valid: {_problem ?? FindProblem()}";
    public bool IsBroken()
    {
        _problem = FindProblem();
        return _problem is not null;
    }

    private string? FindProblem()
    {
        foreach (var argument in _arguments)
        {
            if (FindParameter(argument.Name) is null)
                return $"unknown argument {argument.Name}, the function takes {ParameterNames()}";

            if (_arguments.Count(a => a.Name == argument.Name) > 1)
                return $"argument {argument.Name} is given more than once";
        }

        var missing = _function.Parameters.FirstOrDefault(p => p.Required && _arguments.All(a => a.Name != p.Name));
        if (missing is not null)
            return $"required argument {missing.Name} is missing";

        foreach (var argument in _arguments)
        {
            var problem = FindArgumentProblem(argument, FindParameter(argument.Name)!.Type);
            if (problem is not null)
                return problem;
        }

        return null;
    }

    private PropertyDescription? FindParameter(string name)
    {
        return _function.Parameters.FirstOrDefault(p => p.Name == name);
    }

    private static string? FindArgumentProblem(CallArgument argument, TypeDescription type)
    {
        switch (argument.Kind)
        {
            case CallArgumentKind.Result:
                return null;

            case CallArgumentKind.Text:
                return type.Kind is TypeKind.String or TypeKind.Any
                    ? null
                    : $"{argument.Name}: {{{{result:N}}}} inside a text fits only a text argument, give the whole value as {{{{result:N}}}}";

            default:
                if (CallArgumentParser.ContainsReference(argument.Json))
                    return $"{argument.Name}: {{{{result:N}}}} can be a whole argument or a part of a text argument, "
                           + "not a part of an object or an array";

                using (var document = JsonDocument.Parse(argument.Json))
                    return FindValueProblem(type, document.RootElement, argument.Name);
        }
    }

    // null when the value fits, else "<path>: <problem>"
    private static string? FindValueProblem(TypeDescription type, JsonElement value, string path)
    {
        if (value.ValueKind == JsonValueKind.Null)
            return type.Nullable || type.Kind == TypeKind.Any ? null : $"{path}: null is not allowed";

        return type.Kind switch
        {
            TypeKind.Any => null,
            TypeKind.String => FindStringProblem(type, value, path),
            TypeKind.Integer or TypeKind.Number => FindNumberProblem(type, value, path),
            TypeKind.Boolean => value.ValueKind is JsonValueKind.True or JsonValueKind.False
                ? null
                : $"{path}: expected a boolean",
            TypeKind.Array => FindArrayProblem(type, value, path),
            TypeKind.Object => FindObjectProblem(type, value, path),
            _ => $"{path}: unknown kind"
        };
    }

    private static string? FindStringProblem(TypeDescription type, JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.String)
            return $"{path}: expected a string";

        var text = value.GetString()!;
        var length = text.EnumerateRunes().Count();

        if (type.Enum is not null && !type.Enum.Contains(text, StringComparer.Ordinal))
            return $"{path}: expected one of {string.Join(", ", type.Enum)}";

        if (length < type.MinLength)
            return $"{path}: expected at least {type.MinLength} characters";

        if (length > type.MaxLength)
            return $"{path}: expected at most {type.MaxLength} characters";

        if (type.Pattern is not null && !MatchesPattern(text, type.Pattern))
            return $"{path}: does not match the pattern {type.Pattern}";

        if (!FitsFormat(text, type.Format))
            return $"{path}: expected the format {type.Format}";

        return null;
    }

    private static string? FindNumberProblem(TypeDescription type, JsonElement value, string path)
    {
        var expected = type.Kind == TypeKind.Integer ? "an integer" : "a number";
        if (value.ValueKind != JsonValueKind.Number)
            return $"{path}: expected {expected}";

        if (!value.TryGetDecimal(out var number))
            return $"{path}: the number is out of range";

        if (type.Kind == TypeKind.Integer && number != decimal.Truncate(number))
            return $"{path}: expected an integer";

        if (number < type.Minimum)
            return $"{path}: expected at least {type.Minimum}";

        if (number > type.Maximum)
            return $"{path}: expected at most {type.Maximum}";

        return null;
    }

    private static string? FindArrayProblem(TypeDescription type, JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Array)
            return $"{path}: expected an array";

        var count = value.GetArrayLength();
        if (count < type.MinItems)
            return $"{path}: expected at least {type.MinItems} items";

        if (count > type.MaxItems)
            return $"{path}: expected at most {type.MaxItems} items";

        if (type.Items is null)
            return null;

        var index = 0;
        foreach (var item in value.EnumerateArray())
        {
            var problem = FindValueProblem(type.Items, item, $"{path}[{index++}]");
            if (problem is not null)
                return problem;
        }

        return null;
    }

    private static string? FindObjectProblem(TypeDescription type, JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Object)
            return $"{path}: expected an object";

        var properties = type.Properties ?? [];
        foreach (var property in properties.Where(p => p.Required))
        {
            if (!value.TryGetProperty(property.Name, out _))
                return $"{path}.{property.Name}: required property is missing";
        }

        foreach (var member in value.EnumerateObject())
        {
            var memberPath = $"{path}.{member.Name}";
            var memberType = properties.FirstOrDefault(p => p.Name == member.Name)?.Type ?? type.AdditionalProperties;

            // an object described without properties takes any keys
            if (memberType is null && type.Properties is not null)
                return $"{memberPath}: unknown property";

            var problem = memberType is null ? null : FindValueProblem(memberType, member.Value, memberPath);
            if (problem is not null)
                return problem;
        }

        return null;
    }

    private static bool MatchesPattern(string text, string pattern)
    {
        try
        {
            return Regex.IsMatch(text, pattern, RegexOptions.None, PatternTimeout);
        }
        catch (Exception e) when (e is ArgumentException or RegexMatchTimeoutException)
        {
            return false;
        }
    }

    // only formats with one clear meaning are checked, other formats are a hint for the model
    private static bool FitsFormat(string text, string? format)
    {
        var invariant = CultureInfo.InvariantCulture;
        return format switch
        {
            "uuid" => Guid.TryParse(text, out _),
            "date-time" => DateTimeOffset.TryParse(text, invariant, DateTimeStyles.None, out _),
            "date" => DateOnly.TryParseExact(text, "yyyy-MM-dd", invariant, DateTimeStyles.None, out _),
            "time" => TimeOnly.TryParse(text, invariant, DateTimeStyles.None, out _),
            _ => true
        };
    }

    private string ParameterNames()
    {
        return _function.Parameters.Count == 0
            ? "no arguments"
            : string.Join(", ", _function.Parameters.Select(p => p.Name));
    }
}
