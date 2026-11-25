namespace Dara.Shared.Contracts.StateSnapshots;

public record AgentRunSnapshot(Guid AgentRunId, Guid GroupId, Guid AuthorId, Guid MessageId, string Prompt, string Status, string? FailureReason, IReadOnlyList<PlannedCallSnapshot> Calls);
