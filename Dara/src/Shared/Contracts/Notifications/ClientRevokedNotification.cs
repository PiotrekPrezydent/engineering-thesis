namespace Dara.Shared.Contracts.Notifications;

// the client was revoked in the account panel: the server refuses its token from now on and closes its connections
// right after this notification, so the client signs out
public record ClientRevokedNotification(Guid ClientId);
