using System.Text.Json;
using System.Text.RegularExpressions;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

// keywords fit their kind, limits are in order and arrays say what they hold
public class TypeDescriptionMustBeValidRule : IBuisnessRule
{
    private readonly IReadOnlyList<FunctionDescription> _functions;

    public TypeDescriptionMustBeValidRule(IReadOnlyList<FunctionDescription> functions)
    {
        _functions = functions;
    }

    public string Message => $"Function description is not valid: {FindProblem()}";
    public bool IsBroken()
    {
        return FindProblem() is not null;
    }

    private string? FindProblem()
    {
        if (_functions is null)
            return "functions are missing";

        foreach (var function in _functions)
        {
            if (function is null)
                return "a function is missing";

            if (function.Parameters is null)
                return $"{function.Name}: parameters are missing";

            var problem = CheckProperties(function.Name, function.Parameters);
            if (problem is not null)
                return problem;

            if (function.Returns is not null)
            {
                problem = Check($"{function.Name} returns", function.Returns);
                if (problem is not null)
                    return problem;
            }
        }

        return null;
    }

    private static string? CheckProperties(string path, IReadOnlyList<PropertyDescription> properties)
    {
        foreach (var property in properties)
        {
            if (property?.Type is null)
                return $"{path}: a property without a type";

            var propertyPath = $"{path}.{property.Name}";

            // a default makes the value optional, like a c# parameter with a default
            if (property.Required && property.Type.Default is not null)
                return $"{propertyPath}: required with a default value";

            var problem = Check(propertyPath, property.Type);
            if (problem is not null)
                return problem;
        }

        return null;
    }

    private static string? Check(string path, TypeDescription type)
    {
        var problem = CheckNode(type);
        if (problem is not null)
            return $"{path}: {problem}";

        if (type.Properties is not null)
        {
            problem = CheckProperties(path, type.Properties);
            if (problem is not null)
                return problem;
        }

        if (type.Items is not null)
        {
            problem = Check($"{path}[]", type.Items);
            if (problem is not null)
                return problem;
        }

        if (type.AdditionalProperties is not null)
            return Check($"{path}{{}}", type.AdditionalProperties);

        return null;
    }

    private static string? CheckNode(TypeDescription type)
    {
        var isString = type.Kind == TypeKind.String;
        var isNumber = type.Kind is TypeKind.Integer or TypeKind.Number;
        var isArray = type.Kind == TypeKind.Array;
        var isObject = type.Kind == TypeKind.Object;

        if (!Enum.IsDefined(type.Kind))
            return "unknown kind";

        if (type.Kind == TypeKind.Any && type.Nullable)
            return "any already accepts null";

        if (!isString && (type.Format ?? type.Pattern ?? type.ContentEncoding) is not null)
            return "format, pattern and content encoding are only for strings";

        if (!isString && (type.MinLength ?? type.MaxLength) is not null)
            return "length limits are only for strings";

        if (!isString && type.Enum is not null)
            return "enum is only for strings";

        if (!isNumber && (type.Minimum ?? type.Maximum) is not null)
            return "minimum and maximum are only for numbers";

        if (!isArray && (type.Items is not null || (type.MinItems ?? type.MaxItems) is not null))
            return "items and item limits are only for arrays";

        if (isArray && type.Items is null)
            return "array without items";

        if (!isObject && (type.Properties is not null || type.AdditionalProperties is not null))
            return "properties are only for objects";

        if (type.MinLength < 0 || type.MaxLength < 0 || type.MinItems < 0 || type.MaxItems < 0)
            return "negative limit";

        if (type.MinLength > type.MaxLength || type.MinItems > type.MaxItems || type.Minimum > type.Maximum)
            return "lower limit above upper limit";

        if (type.Enum is { Count: 0 })
            return "empty enum";

        if (type.Enum is not null && type.Enum.Distinct(StringComparer.Ordinal).Count() != type.Enum.Count)
            return "repeated enum value";

        if (type.Pattern is not null && !IsRegex(type.Pattern))
            return "pattern is not a regular expression";

        if (type.Default is not null && !IsJson(type.Default))
            return "default is not json";

        return null;
    }

    private static bool IsRegex(string pattern)
    {
        try
        {
            _ = new Regex(pattern);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool IsJson(string text)
    {
        try
        {
            using var _ = JsonDocument.Parse(text);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
