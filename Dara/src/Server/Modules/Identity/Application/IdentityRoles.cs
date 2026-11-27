using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Identity.Application;

public static class IdentityRoles
{
    private const string OwnerCode = "Identity.Owner";
    private const string ClientOwnerCode = "Identity.ClientOwner";

    // sees the clients of the account
    public static readonly RoleDefinition Viewer = new("Identity.Viewer", IdentityScopes.User, [IdentityPermissions.ReadClients]);

    // the account holder: everything a viewer may, manages the clients and who has which role on the account
    public static readonly RoleDefinition Owner = new(OwnerCode, IdentityScopes.User, [IdentityPermissions.ManageClients, IdentityPermissions.ManageAccess])
    {
        Subordinates = [Viewer],
        ManageableRoleCodes = [OwnerCode, Viewer.Code]
    };

    // holds one client: the client itself and its user
    public static readonly RoleDefinition ClientOwner = new(ClientOwnerCode, IdentityScopes.Client, [IdentityPermissions.ChangeClientContext, IdentityPermissions.ManageClientAccess])
    {
        ManageableRoleCodes = [ClientOwnerCode]
    };

    // a new account holds itself, a new client holds itself (its user gets ClientOwner on it too, as the one who added it)
    public static readonly IReadOnlyList<NewSubjectRole> ForNewUser = [new(Owner, OnItself: true)];
    public static readonly IReadOnlyList<NewSubjectRole> ForNewClient = [new(ClientOwner, OnItself: true)];
}
