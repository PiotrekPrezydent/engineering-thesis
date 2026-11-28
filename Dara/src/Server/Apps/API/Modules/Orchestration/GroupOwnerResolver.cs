using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;

namespace Dara.Server.Apps.API.Modules.Orchestration;

// until TASK-ENG-030 a group is owned by a client: the panel acts as the owner client of the group when that client
// belongs to its account; a group of another account looks missing (404). TASK-ENG-030 gives the group an owner
// account and removes this class
public class GroupOwnerResolver
{
    private readonly IGroupsModule _groups;
    private readonly IIdentityModule _identity;

    public GroupOwnerResolver(IGroupsModule groups, IIdentityModule identity)
    {
        _groups = groups;
        _identity = identity;
    }

    public async Task<Guid> OwnerClientOfAsync(ClaimsPrincipal user, Guid groupId)
    {
        var group = await _groups.ExecuteQueryAsync<GetGroupDetailsQuery, GroupDetailsDto>(new GetGroupDetailsQuery(groupId));
        var owner = await _identity.ExecuteQueryAsync<GetAccountClientQuery, UserClientDto?>(new GetAccountClientQuery(user.UserId(), group.OwnerId));

        return owner?.ClientId ?? throw new EntityNotFoundException("Group", groupId);
    }
}
