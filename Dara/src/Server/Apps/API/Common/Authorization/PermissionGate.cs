using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.IsAuthorized;

namespace Dara.Server.Apps.API.Common.Authorization;

internal static class PermissionGate
{
    // default deny; the sender is whoever calls: a client by its token, a user by the panel cookie, each with only its
    // own roles; the host asks Authorization nothing but IsAuthorized(sender, permission, resource)
    public static async Task<bool> IsAllowedAsync(ClaimsPrincipal user, IAuthorizationModule authorization, PermissionDefinition permission, Guid scopeId)
    {
        if (!user.IsClient() && !user.IsAccount())
            return false;

        return await authorization.ExecuteQueryAsync<IsAuthorizedQuery, bool>(new IsAuthorizedQuery(user.SenderId(), permission.Code, scopeId));
    }
}
