namespace Dara.Shared.Contracts.Notifications;

public record ExecutionPlanGenerationFailedNotification(Guid AgentRunId, Guid GroupId, string Reason);
