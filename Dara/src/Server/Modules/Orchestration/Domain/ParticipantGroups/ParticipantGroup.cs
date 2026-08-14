using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;

public class ParticipantGroup : Entity, IAggregateRoot
{
    public ParticipantGroupId Id { get; private set; }

    public IReadOnlyList<ParticipantId> Participants => _participants.AsReadOnly();
    
    private List<ParticipantId> _participants;

    private ParticipantGroup()
    {
    }

    private ParticipantGroup(ParticipantGroupId id, ParticipantId creator)
    {
        Id = id;
        _participants = new List<ParticipantId> { creator };
    }

    public static ParticipantGroup Create(ParticipantGroupId id, ParticipantId creator)
    {
        return new ParticipantGroup(id, creator);
    }

    public void AddParticipant(ParticipantId participant)
    {
        _participants.Add(participant);
    }
    
    public void RemoveParticipant(ParticipantId participant)
    {
        _participants.Remove(participant);
    }
}