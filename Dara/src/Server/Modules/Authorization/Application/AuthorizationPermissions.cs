using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Authorization.Application;

public static class AuthorizationPermissions
{
    // grants and revokes every role on every resource through the api, above the rules of the scopes
    public static readonly PermissionDefinition Administer = new("authorization.server.administer", AuthorizationScopes.Server);
}
