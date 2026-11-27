using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Authorization.Application;

// Authorization controls the server as a whole: its administrator; who may grant and revoke the other roles decides
// the module of their scope (the permission that manages access and the manageable roles of the held roles)
internal sealed class AuthorizationAuthorizationDefinition : IModuleAuthorizationDefinition
{
    public string ModuleName => "Authorization";

    public IEnumerable<ScopeDefinition> GetScopes() => [AuthorizationScopes.Server];

    public IEnumerable<PermissionDefinition> GetPermissions() => [AuthorizationPermissions.Administer];

    public IEnumerable<RoleDefinition> GetDefaultRoles() => [AuthorizationRoles.ServerAdmin];
}
