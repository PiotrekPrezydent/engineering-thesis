namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments;

public interface IRoleAssignmentChecker
{
    bool IsAssigned(Guid subjectId, string roleCode, Guid scopeId);
}
