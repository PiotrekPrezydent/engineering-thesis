namespace Dara.Shared.Contracts.Notifications;

public record GroupMessageReceivedNotification(Guid GroupId, Guid MessageId, Guid MessageAuthorId, string Message);