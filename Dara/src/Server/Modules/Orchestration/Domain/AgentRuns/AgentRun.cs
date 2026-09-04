using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

public class AgentRun : Entity, IAggregateRoot
{
    public AgentRunId Id { get; private set; }
    
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public AgentRunStatus Status { get; private set; }
    
    public GroupSnapshot GroupSnapshot { get; private set; }
    public IReadOnlyList<ParticipantSnapshot> ParticipantsSnapshots { get; private set; }
    
    private AgentRun(AgentRunStatus status, GroupSnapshot groupSnapshot, IReadOnlyList<ParticipantSnapshot> participantsSnapshots)
    {
        Id = new AgentRunId(Guid.NewGuid());
        Status = status;
        GroupSnapshot = groupSnapshot;
        ParticipantsSnapshots = participantsSnapshots;
    }
    
    public AgentRun CreateNew(GroupId groupId, GroupSnapshot groupSnapshot, IReadOnlyList<ParticipantSnapshot> participantsSnapshots)
    {
        return new AgentRun(AgentRunStatus.Starting, groupSnapshot, participantsSnapshots);
    }

    public void StartRunning()
    {
        Status = AgentRunStatus.Running;
    }

    public void SetCompleted()
    {
        Status = AgentRunStatus.Completed;
    }
}