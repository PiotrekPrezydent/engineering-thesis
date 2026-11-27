using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Identity.Application;

// the authorization definition of the module, checked at start-up and synchronized into Authorization
internal sealed class IdentityAuthorizationDefinition : IModuleAuthorizationDefinition
{
    public string ModuleName => "Identity";

    public IEnumerable<ScopeDefinition> GetScopes() => [IdentityScopes.User, IdentityScopes.Client];

    public IEnumerable<PermissionDefinition> GetPermissions() => [IdentityPermissions.ReadClients, IdentityPermissions.ManageClients, IdentityPermissions.ManageAccess, IdentityPermissions.ChangeClientContext, IdentityPermissions.ManageClientAccess];

    public IEnumerable<RoleDefinition> GetDefaultRoles() => [IdentityRoles.Owner, IdentityRoles.Viewer, IdentityRoles.ClientOwner];

    public IEnumerable<NewSubjectRole> GetNewPanelRoles() => IdentityRoles.ForNewPanel;

    public IEnumerable<NewSubjectRole> GetNewClientRoles() => IdentityRoles.ForNewClient;
}
