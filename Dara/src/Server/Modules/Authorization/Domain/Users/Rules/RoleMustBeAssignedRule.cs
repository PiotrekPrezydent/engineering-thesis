using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users.Rules;

// only a role the account or the client has can be removed
public class RoleMustBeAssignedRule : IBuisnessRule
{
    private readonly bool _assigned;

    public RoleMustBeAssignedRule(bool assigned)
    {
        _assigned = assigned;
    }

    public string Message => "The role is not assigned";

    public bool IsBroken()
    {
        return !_assigned;
    }
}
