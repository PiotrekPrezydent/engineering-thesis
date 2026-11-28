using Dara.Server.Apps.API.Common.Errors;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups.Rules;

namespace Dara.Server.Apps.API.Modules.Groups;

// rules of groups that are no validation: who may write or join (403), a repeated join or leave (409)
public static class GroupsProblems
{
    public static IResult? From(IBuisnessRule rule)
    {
        return rule switch
        {
            OnlyActualGroupMemberCanSendMessageRule or ProvidedGroupJoiningCodeMustBeValidRule => DomainProblems.Forbidden(rule),
            GroupMemberCannotBeAddedTwiceRule or NonActualMemberCannotLeaveGroupRule => DomainProblems.Conflict(rule),
            _ => null
        };
    }
}
