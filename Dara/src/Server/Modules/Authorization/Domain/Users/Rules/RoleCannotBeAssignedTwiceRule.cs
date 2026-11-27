using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users.Rules;

// an account or a client has a role once
public class RoleCannotBeAssignedTwiceRule : IBuisnessRule
{
    private readonly bool _assigned;

    public RoleCannotBeAssignedTwiceRule(bool assigned)
    {
        _assigned = assigned;
    }

    public string Message => "The role is already assigned";

    public bool IsBroken()
    {
        return _assigned;
    }
}
