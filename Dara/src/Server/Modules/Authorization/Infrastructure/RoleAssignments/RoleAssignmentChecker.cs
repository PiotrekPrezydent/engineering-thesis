using Dara.Server.Modules.Authorization.Domain.RoleAssignments;

namespace Dara.Server.Modules.Authorization.Infrastructure.RoleAssignments;

public class RoleAssignmentChecker : IRoleAssignmentChecker
{
    private readonly AuthorizationContext _context;

    public RoleAssignmentChecker(AuthorizationContext context)
    {
        _context = context;
    }

    public bool IsAssigned(Guid subjectId, string roleCode, Guid scopeId)
    {
        return _context.RoleAssignments.Any(a => a.SubjectId == subjectId && a.RoleCode == roleCode && a.ScopeId == scopeId);
    }
}
