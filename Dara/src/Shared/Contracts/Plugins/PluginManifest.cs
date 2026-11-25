using Dara.Shared.Contracts.Functions;

namespace Dara.Shared.Contracts.Plugins;

// what a client registers for one plugin; sha256 is the hex hash of the plugin .dll counted by the client
public record PluginManifest(
    string Name,
    string? Description,
    string Version,
    string Sha256,
    IReadOnlyList<FunctionDescription> Functions);
