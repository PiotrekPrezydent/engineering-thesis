using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;

// a grant or revoke passes along only the companion roles of the role (its subordinates of a child scope)
public class CompanionMustBelongToTheRoleRule : IBuisnessRule
{
    private readonly Role _role;
    private readonly string _companionRoleCode;

    public CompanionMustBelongToTheRoleRule(Role role, string companionRoleCode)
    {
        _role = role;
        _companionRoleCode = companionRoleCode;
    }

    public string Message => $"{_companionRoleCode} is no companion role of {_role.Code}";

    public bool IsBroken()
    {
        return !_role.CompanionRoleCodes.Contains(_companionRoleCode);
    }
}
