namespace Dara.Shared.Contracts.Notifications;

public record AgentRunStartedNotification(Guid AgentRunId, Guid GroupId, Guid MessageId);
