using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;
using Contract = Dara.Shared.Contracts.Functions;

namespace Dara.Server.Modules.Plugins.Application.Plugins;

// the client contract and the domain describe a function in the same shape, field by field
public static class PluginManifestMapper
{
    public static FunctionDescription ToDomain(Contract.FunctionDescription function)
    {
        return new FunctionDescription(
            function.Name,
            function.Description,
            function.Parameters.Select(ToDomain).ToList(),
            function.Returns is null ? null : ToDomain(function.Returns));
    }

    public static Contract.FunctionDescription ToContract(FunctionDescription function)
    {
        return new Contract.FunctionDescription(
            function.Name,
            function.Description,
            function.Parameters.Select(ToContract).ToList(),
            function.Returns is null ? null : ToContract(function.Returns));
    }

    private static PropertyDescription ToDomain(Contract.PropertyDescription property)
    {
        return new PropertyDescription(property.Name, ToDomain(property.Type), property.Required);
    }

    private static Contract.PropertyDescription ToContract(PropertyDescription property)
    {
        return new Contract.PropertyDescription(property.Name, ToContract(property.Type), property.Required);
    }

    private static TypeDescription ToDomain(Contract.TypeDescription type)
    {
        return new TypeDescription(
            ToDomain(type.Kind),
            type.Nullable,
            type.Description,
            type.Format,
            type.Pattern,
            type.MinLength,
            type.MaxLength,
            type.Minimum,
            type.Maximum,
            type.MinItems,
            type.MaxItems,
            type.Enum?.ToList(),
            type.Default,
            type.ContentEncoding,
            type.Items is null ? null : ToDomain(type.Items),
            type.Properties?.Select(ToDomain).ToList(),
            type.AdditionalProperties is null ? null : ToDomain(type.AdditionalProperties));
    }

    private static Contract.TypeDescription ToContract(TypeDescription type)
    {
        return new Contract.TypeDescription(
            ToContract(type.Kind),
            type.Nullable,
            type.Description,
            type.Format,
            type.Pattern,
            type.MinLength,
            type.MaxLength,
            type.Minimum,
            type.Maximum,
            type.MinItems,
            type.MaxItems,
            type.Enum?.ToList(),
            type.Default,
            type.ContentEncoding,
            type.Items is null ? null : ToContract(type.Items),
            type.Properties?.Select(ToContract).ToList(),
            type.AdditionalProperties is null ? null : ToContract(type.AdditionalProperties));
    }

    private static TypeKind ToDomain(Contract.TypeKind kind)
    {
        return kind switch
        {
            Contract.TypeKind.String => TypeKind.String,
            Contract.TypeKind.Integer => TypeKind.Integer,
            Contract.TypeKind.Number => TypeKind.Number,
            Contract.TypeKind.Boolean => TypeKind.Boolean,
            Contract.TypeKind.Object => TypeKind.Object,
            Contract.TypeKind.Array => TypeKind.Array,
            Contract.TypeKind.Any => TypeKind.Any,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown type kind")
        };
    }

    private static Contract.TypeKind ToContract(TypeKind kind)
    {
        return kind switch
        {
            TypeKind.String => Contract.TypeKind.String,
            TypeKind.Integer => Contract.TypeKind.Integer,
            TypeKind.Number => Contract.TypeKind.Number,
            TypeKind.Boolean => Contract.TypeKind.Boolean,
            TypeKind.Object => Contract.TypeKind.Object,
            TypeKind.Array => Contract.TypeKind.Array,
            TypeKind.Any => Contract.TypeKind.Any,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown type kind")
        };
    }
}
