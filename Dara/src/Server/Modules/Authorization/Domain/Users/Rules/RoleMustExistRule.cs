using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Domain.Users.Rules;

// only a role some module declares can be given
public class RoleMustExistRule : IBuisnessRule
{
    private readonly string _roleCode;
    private readonly IRoleExistenceChecker _checker;

    public RoleMustExistRule(string roleCode, IRoleExistenceChecker checker)
    {
        _roleCode = roleCode;
        _checker = checker;
    }

    public string Message => "The role does not exist";

    public bool IsBroken()
    {
        return !_checker.Exists(_roleCode);
    }
}
