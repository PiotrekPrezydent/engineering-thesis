using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.Groups.Rules;

public class AuthorMustBeGroupParticipantRule : IBuisnessRule
{
    private readonly IReadOnlyList<ParticipantId> _participants;
    private readonly ParticipantId _authorId;

    public AuthorMustBeGroupParticipantRule(IReadOnlyList<ParticipantId> participants, ParticipantId authorId)
    {
        _participants = participants;
        _authorId = authorId;
    }

    public string Message => "Only a participant of the group can start an agent run in it";
    public bool IsBroken()
    {
        return !_participants.Contains(_authorId);
    }
}
