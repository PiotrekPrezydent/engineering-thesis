using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

// full description of a value as the plugin gave it, null means the plugin said nothing about it
public record TypeDescription(
    TypeKind Kind,
    bool Nullable = false,
    string? Description = null,
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
    TypeDescription? Items = null,
    IReadOnlyList<PropertyDescription>? Properties = null,
    TypeDescription? AdditionalProperties = null) : IValueObject;
