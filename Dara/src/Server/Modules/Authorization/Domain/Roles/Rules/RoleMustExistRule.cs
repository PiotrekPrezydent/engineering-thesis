using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Roles.Rules;

// only a role some module declares can be granted or revoked
public class RoleMustExistRule : IBuisnessRule
{
    private readonly Role? _role;

    public RoleMustExistRule(Role? role)
    {
        _role = role;
    }

    public string Message => "The role does not exist";

    public bool IsBroken()
    {
        return _role is null;
    }
}
