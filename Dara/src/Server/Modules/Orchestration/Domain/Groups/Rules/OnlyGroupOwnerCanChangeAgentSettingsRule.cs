using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.Groups.Rules;

// the api key and the other agent settings belong to the group owner
public class OnlyGroupOwnerCanChangeAgentSettingsRule : IBuisnessRule
{
    private readonly ParticipantId _ownerId;
    private readonly ParticipantId _editorId;

    public OnlyGroupOwnerCanChangeAgentSettingsRule(ParticipantId ownerId, ParticipantId editorId)
    {
        _ownerId = ownerId;
        _editorId = editorId;
    }

    public string Message => "Only the group owner can change the agent settings of the group";
    public bool IsBroken()
    {
        return _ownerId != _editorId;
    }
}
