namespace Dara.Shared.Contracts.Notifications;

public record ProfileNameChangedNotification(Guid ProfileId, string ProfileName);