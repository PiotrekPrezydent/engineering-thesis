using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;

// an assignment on every resource of a scope comes only from the events of modules and the seed, never from the api
public class GlobalRoleIsAssignedOnlyAutomaticallyRule : IBuisnessRule
{
    private readonly bool _isGlobal;

    public GlobalRoleIsAssignedOnlyAutomaticallyRule(bool isGlobal)
    {
        _isGlobal = isGlobal;
    }

    public string Message => "A role on every resource of a scope is granted and revoked only automatically";

    public bool IsBroken()
    {
        return _isGlobal;
    }
}
