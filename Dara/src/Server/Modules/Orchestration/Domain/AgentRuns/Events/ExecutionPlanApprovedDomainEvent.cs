using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

public record ExecutionPlanApprovedDomainEvent(AgentRunId AgentRunId, GroupId GroupId, ParticipantId AuthorId) : DomainEventBase;
