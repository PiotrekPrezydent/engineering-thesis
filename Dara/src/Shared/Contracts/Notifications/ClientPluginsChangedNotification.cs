namespace Dara.Shared.Contracts.Notifications;

// only a signal with the version of the installations of the client: the client reads its installations over http and
// fetches missing packages (TASK-ENG-031 sends it, TASK-ENG-034 handles it)
public record ClientPluginsChangedNotification(Guid ClientId, int Version);
