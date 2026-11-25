using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts.Notifications;

// the plan waits for the decision of its author
public record ExecutionPlanCreatedNotification(Guid AgentRunId, Guid GroupId, IReadOnlyList<PlannedCallSnapshot> Calls);
