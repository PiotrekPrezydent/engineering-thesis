using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;

// only a role the subject has on the resource can be revoked
public class RoleMustBeAssignedRule : IBuisnessRule
{
    private readonly RoleAssignment? _assignment;

    public RoleMustBeAssignedRule(RoleAssignment? assignment)
    {
        _assignment = assignment;
    }

    public string Message => "The subject does not have the role on this resource";

    public bool IsBroken()
    {
        return _assignment is null;
    }
}
