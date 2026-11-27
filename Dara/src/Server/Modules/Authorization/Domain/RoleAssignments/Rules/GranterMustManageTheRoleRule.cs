using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;

// no escalation: a subject grants or revokes a role only when a role it holds on that resource lists it among its
// manageable roles (an owner all of the scope, a manager all but the owner)
public class GranterMustManageTheRoleRule : IBuisnessRule
{
    private readonly GranterAccess _granter;
    private readonly Role _role;

    public GranterMustManageTheRoleRule(GranterAccess granter, Role role)
    {
        _granter = granter;
        _role = role;
    }

    public string Message => "No role of the caller on this resource manages the role";

    public bool IsBroken()
    {
        return !_granter.Manages(_role.Code);
    }
}
