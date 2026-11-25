namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;

public record AgentRunDto(
    Guid AgentRunId,
    Guid GroupId,
    Guid AuthorId,
    Guid MessageId,
    string Prompt,
    string Status,
    string? FailureReason,
    List<PlannedCallDto> Calls);

public record PlannedCallDto(int Index, string Alias, Guid ParticipantId, string ParticipantName, Guid FunctionId, string FunctionName, string ArgumentsJson);
