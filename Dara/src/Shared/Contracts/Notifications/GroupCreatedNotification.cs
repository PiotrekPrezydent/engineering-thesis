namespace Dara.Shared.Contracts.Notifications;

public record GroupCreatedNotification(Guid GroupId, string Name, string JoinCode);