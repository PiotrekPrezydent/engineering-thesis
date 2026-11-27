using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Identity.Application;

// what a subject may do on one resource of a scope of the module, the code starts with the path of the scope; the
// endpoints of the module are marked with them once the owner decides who may do what (TASK-ENG-040-D08)
public static class IdentityPermissions
{
    public static readonly PermissionDefinition ReadClients = new("identity.user.read_clients", IdentityScopes.User);
    public static readonly PermissionDefinition ManageClients = new("identity.user.manage_clients", IdentityScopes.User);
    public static readonly PermissionDefinition ManageAccess = new(IdentityScopes.User.ManageAccessPermissionCode, IdentityScopes.User);
    public static readonly PermissionDefinition ChangeClientContext = new("identity.client.change_context", IdentityScopes.Client);
    public static readonly PermissionDefinition ManageClientAccess = new(IdentityScopes.Client.ManageAccessPermissionCode, IdentityScopes.Client);
}
