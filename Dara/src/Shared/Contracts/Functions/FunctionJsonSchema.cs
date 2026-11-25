using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dara.Shared.Contracts.Functions;

// json schema in the shape microsoft.extensions.ai gives from reflection, null as a type array member
public static class FunctionJsonSchema
{
    public static JsonElement CreateParametersSchema(FunctionDescription function)
    {
        var root = new JsonObject { ["type"] = "object" };
        AddProperties(root, function.Parameters);

        return ToElement(root);
    }

    // null for void functions
    public static JsonElement? CreateReturnSchema(FunctionDescription function)
    {
        return function.Returns is null ? null : ToElement(CreateNode(function.Returns));
    }

    public static JsonNode CreateNode(TypeDescription type)
    {
        var node = new JsonObject();

        if (type.Description is not null)
            node["description"] = type.Description;

        if (type.Kind != TypeKind.Any)
            node["type"] = type.Nullable ? new JsonArray(KindName(type.Kind), "null") : KindName(type.Kind);

        if (type.Format is not null)
            node["format"] = type.Format;

        if (type.Pattern is not null)
            node["pattern"] = type.Pattern;

        if (type.MinLength is not null)
            node["minLength"] = type.MinLength;

        if (type.MaxLength is not null)
            node["maxLength"] = type.MaxLength;

        if (type.Minimum is not null)
            node["minimum"] = type.Minimum;

        if (type.Maximum is not null)
            node["maximum"] = type.Maximum;

        if (type.Enum is not null)
            node["enum"] = CreateEnum(type.Enum, type.Nullable);

        if (type.Properties is not null)
            AddProperties(node, type.Properties);

        if (type.AdditionalProperties is not null)
            node["additionalProperties"] = CreateNode(type.AdditionalProperties);

        if (type.Items is not null)
            node["items"] = CreateNode(type.Items);

        if (type.MinItems is not null)
            node["minItems"] = type.MinItems;

        if (type.MaxItems is not null)
            node["maxItems"] = type.MaxItems;

        if (type.ContentEncoding is not null)
            node["contentEncoding"] = type.ContentEncoding;

        if (type.Default is not null)
            node["default"] = JsonNode.Parse(type.Default);

        // a node without any keyword accepts every value
        return node.Count == 0 ? JsonValue.Create(true) : node;
    }

    public static string KindName(TypeKind kind)
    {
        return kind switch
        {
            TypeKind.String => "string",
            TypeKind.Integer => "integer",
            TypeKind.Number => "number",
            TypeKind.Boolean => "boolean",
            TypeKind.Object => "object",
            TypeKind.Array => "array",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "kind has no json schema type")
        };
    }

    private static void AddProperties(JsonObject node, IReadOnlyList<PropertyDescription> properties)
    {
        var schemas = new JsonObject();
        var required = new JsonArray();

        foreach (var property in properties)
        {
            schemas[property.Name] = CreateNode(property.Type);

            if (property.Required)
                required.Add(property.Name);
        }

        node["properties"] = schemas;

        if (required.Count > 0)
            node["required"] = required;
    }

    private static JsonArray CreateEnum(IReadOnlyList<string> values, bool nullable)
    {
        var array = new JsonArray();
        foreach (var value in values)
            array.Add(value);

        if (nullable)
            array.Add(null);

        return array;
    }

    private static JsonElement ToElement(JsonNode node)
    {
        return JsonSerializer.SerializeToElement(node);
    }
}
