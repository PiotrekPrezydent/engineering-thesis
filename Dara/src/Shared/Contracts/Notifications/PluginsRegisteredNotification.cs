using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts.Notifications;

// the whole registered set of the owner after a change of one of its plugins
public record PluginsRegisteredNotification(IReadOnlyList<PluginSnapshot> Plugins);
