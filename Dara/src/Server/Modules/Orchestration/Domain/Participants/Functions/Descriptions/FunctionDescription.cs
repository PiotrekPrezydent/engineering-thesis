using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

// what the model is told about a participant function, null returns mean void
public record FunctionDescription(
    string Name,
    string? Description,
    IReadOnlyList<PropertyDescription> Parameters,
    TypeDescription? Returns) : IValueObject;
