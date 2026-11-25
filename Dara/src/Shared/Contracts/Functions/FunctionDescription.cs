namespace Dara.Shared.Contracts.Functions;

// published language for a plugin function, null returns mean void
public record FunctionDescription(
    string Name,
    string? Description,
    IReadOnlyList<PropertyDescription> Parameters,
    TypeDescription? Returns);
