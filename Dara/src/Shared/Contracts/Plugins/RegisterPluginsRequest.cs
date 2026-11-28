namespace Dara.Shared.Contracts.Plugins;

// the whole set of the plugins of the client replaces the registered one; until TASK-ENG-031 (installations)
public record RegisterPluginsRequest(IReadOnlyList<PluginManifest> Plugins);
