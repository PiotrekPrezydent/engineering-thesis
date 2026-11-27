using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;

// only a subject with the permission that manages access in the scope ("<scope path>.manage_access"), on that
// resource, grants and revokes its roles
public class GranterMustManageAccessRule : IBuisnessRule
{
    private readonly GranterAccess _granter;

    public GranterMustManageAccessRule(GranterAccess granter)
    {
        _granter = granter;
    }

    public string Message => "The caller does not manage access to this resource";

    public bool IsBroken()
    {
        return !_granter.ManagesAccess;
    }
}
