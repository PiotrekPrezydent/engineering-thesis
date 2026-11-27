using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;

public class RoleCannotBeAssignedTwiceRule : IBuisnessRule
{
    private readonly Guid _subjectId;
    private readonly string _roleCode;
    private readonly Guid _scopeId;
    private readonly IRoleAssignmentChecker _checker;

    public RoleCannotBeAssignedTwiceRule(Guid subjectId, string roleCode, Guid scopeId, IRoleAssignmentChecker checker)
    {
        _subjectId = subjectId;
        _roleCode = roleCode;
        _scopeId = scopeId;
        _checker = checker;
    }

    public string Message => "The subject has the role on this resource already";

    public bool IsBroken()
    {
        return _checker.IsAssigned(_subjectId, _roleCode, _scopeId);
    }
}
