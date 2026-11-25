using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

// everything a plugin says about one of its functions, null returns mean void
public record FunctionDescription(
    string Name,
    string? Description,
    IReadOnlyList<PropertyDescription> Parameters,
    TypeDescription? Returns) : IValueObject;
