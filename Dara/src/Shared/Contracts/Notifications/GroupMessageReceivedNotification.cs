namespace Dara.Shared.Contracts.Notifications;

public record GroupMessageReceivedNotification(Guid GroupId, Guid MessageAuthorId, string Message);