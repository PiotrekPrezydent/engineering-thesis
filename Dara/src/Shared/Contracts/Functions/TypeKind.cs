using System.Text.Json.Serialization;

namespace Dara.Shared.Contracts.Functions;

// json schema "type" of a node, any is the schema "true" without a type
[JsonConverter(typeof(JsonStringEnumConverter<TypeKind>))]
public enum TypeKind
{
    String,
    Integer,
    Number,
    Boolean,
    Object,
    Array,
    Any
}
