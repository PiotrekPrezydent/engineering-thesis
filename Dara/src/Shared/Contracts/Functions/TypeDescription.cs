namespace Dara.Shared.Contracts.Functions;

// one node of a json schema, every field is one keyword and null leaves the keyword out
public record TypeDescription(
    TypeKind Kind,
    // adds "null" to "type" and, with enum, to the enum values
    bool Nullable = false,
    string? Description = null,
    // free text like uuid, date-time, date, time, uri, email
    string? Format = null,
    string? Pattern = null,
    int? MinLength = null,
    int? MaxLength = null,
    decimal? Minimum = null,
    decimal? Maximum = null,
    int? MinItems = null,
    int? MaxItems = null,
    IReadOnlyList<string>? Enum = null,
    // json text of the default value, "null" is a null default
    string? Default = null,
    string? ContentEncoding = null,
    // element of an array
    TypeDescription? Items = null,
    // fields of an object
    IReadOnlyList<PropertyDescription>? Properties = null,
    // values of an object used as a dictionary, keys are always text
    TypeDescription? AdditionalProperties = null);
