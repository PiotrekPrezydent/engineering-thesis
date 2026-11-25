using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

public record StartAgentRunCommand(Guid MessageId, Guid GroupId, Guid AuthorId, string Message) : ICommand;
