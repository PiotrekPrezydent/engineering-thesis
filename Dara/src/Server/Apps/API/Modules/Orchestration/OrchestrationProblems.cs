using Dara.Server.Apps.API.Common.Errors;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;
using Dara.Server.Modules.Orchestration.Domain.Groups.Rules;

namespace Dara.Server.Apps.API.Modules.Orchestration;

// rules of orchestration that are no validation: the owner edits the settings, the author decides the plan (403),
// a decided plan (409)
public static class OrchestrationProblems
{
    public static IResult? From(IBuisnessRule rule)
    {
        return rule switch
        {
            OnlyGroupOwnerCanChangeAgentSettingsRule or OnlyAuthorCanDecideAboutPlanRule or AuthorMustBeGroupParticipantRule => DomainProblems.Forbidden(rule),
            AgentRunMustBeInStatusRule => DomainProblems.Conflict(rule),
            _ => null
        };
    }
}
