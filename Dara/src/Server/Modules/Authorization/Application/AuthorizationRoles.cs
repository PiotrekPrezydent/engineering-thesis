using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Authorization.Application;

public static class AuthorizationRoles
{
    // the administrator of the server, the only role above all others; assigned by the seed with ScopeIds.All, nobody
    // grants or revokes it through the api
    public static readonly RoleDefinition ServerAdmin = new("Authorization.ServerAdmin", AuthorizationScopes.Server, [AuthorizationPermissions.Administer]);
}
