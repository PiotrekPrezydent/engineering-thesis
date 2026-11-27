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

    // holds one client: the client itself and the panel client of its account
    public static readonly RoleDefinition ClientOwner = new(ClientOwnerCode, IdentityScopes.Client, [IdentityPermissions.ChangeClientContext, IdentityPermissions.ManageClientAccess])
    {
        ManageableRoleCodes = [ClientOwnerCode]
    };

    // the panel client of a new account gets Identity.Owner on the account (published by Identity itself, the scope is
    // the account, not the panel); a new client holds itself (the panel of its account gets ClientOwner on it too)
    public static readonly IReadOnlyList<NewSubjectRole> ForNewPanel = [];
    public static readonly IReadOnlyList<NewSubjectRole> ForNewClient = [new(ClientOwner, OnItself: true)];
}
