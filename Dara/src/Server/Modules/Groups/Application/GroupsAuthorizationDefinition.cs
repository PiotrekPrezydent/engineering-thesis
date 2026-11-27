using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Groups.Application;

// the authorization definition of the module, checked at start-up and synchronized into Authorization
internal sealed class GroupsAuthorizationDefinition : IModuleAuthorizationDefinition
{
    public string ModuleName => "Groups";

    public IEnumerable<ScopeDefinition> GetScopes() => [GroupsScopes.Group];

    public IEnumerable<PermissionDefinition> GetPermissions() => [GroupsPermissions.Read, GroupsPermissions.SendMessage, GroupsPermissions.ManageMembers, GroupsPermissions.ManageAccess];

    public IEnumerable<RoleDefinition> GetDefaultRoles() => [GroupsRoles.Owner, GroupsRoles.Member, GroupsRoles.Reader];
}
