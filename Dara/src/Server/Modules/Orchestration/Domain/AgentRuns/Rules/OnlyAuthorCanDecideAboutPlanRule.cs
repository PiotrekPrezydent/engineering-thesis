using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// the author approves or rejects the whole plan
public class OnlyAuthorCanDecideAboutPlanRule : IBuisnessRule
{
    private readonly ParticipantId _authorId;
    private readonly ParticipantId _deciderId;

    public OnlyAuthorCanDecideAboutPlanRule(ParticipantId authorId, ParticipantId deciderId)
    {
        _authorId = authorId;
        _deciderId = deciderId;
    }

    public string Message => "Only the author of the prompt can approve or reject its plan";
    public bool IsBroken()
    {
        return _authorId != _deciderId;
    }
}
