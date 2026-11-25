namespace Dara.Shared.Contracts.StateSnapshots;

public record PluginSnapshot(Guid PluginId, string Name, string? Description, string Version, string Sha256, bool IsEnabled, IReadOnlyList<PluginFunctionSnapshot> Functions);
