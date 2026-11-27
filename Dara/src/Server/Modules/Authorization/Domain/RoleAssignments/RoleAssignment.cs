using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments;

// a subject (any id: a user, a client, a group) holds a role on one resource of the scope of the role (ScopeId, e.g.
// the id of a plugin) or on all of them (AllResources); the only aggregate of Authorization that changes after
// start-up; access is denied unless an assignment allows it
public class RoleAssignment : Entity, IAggregateRoot
{
    // "*", the same value as ScopeIds.All of the integration events: a global role, on every resource of its scope
    public static readonly Guid AllResources = Guid.Empty;

    public RoleAssignmentId Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public string RoleCode { get; private set; }
    public string ScopeCode { get; private set; }
    public Guid ScopeId { get; private set; }

    private RoleAssignment() { }

    private RoleAssignment(Guid subjectId, Role role, Guid scopeId)
    {
        Id = new RoleAssignmentId(Guid.NewGuid());
        SubjectId = subjectId;
        RoleCode = role.Code;
        ScopeCode = role.ScopeCode;
        ScopeId = scopeId;
    }

    // automatically: by a module after its own command or an event (an integration event), or by the seed; nobody
    // grants it, so it may be global
    public static RoleAssignment Assign(Guid subjectId, Role role, Guid scopeId)
    {
        return new RoleAssignment(subjectId, role, scopeId);
    }

    // by a subject through the api: on one resource, where it manages access and holds a role that manages this role
    public static RoleAssignment Grant(Guid subjectId, Role role, Guid scopeId, GranterAccess granter, IRoleAssignmentChecker checker)
    {
        var assignment = new RoleAssignment(subjectId, role, scopeId);
        assignment.CheckGranter(granter, role);
        assignment.CheckRule(new RoleCannotBeAssignedTwiceRule(subjectId, role.Code, scopeId, checker));
        return assignment;
    }

    // by a subject through the api, on the same terms as a grant; the repository removes it afterwards
    public void Revoke(GranterAccess granter, Role role)
    {
        CheckGranter(granter, role);
    }

    private void CheckGranter(GranterAccess granter, Role role)
    {
        CheckRule(new GlobalRoleIsAssignedOnlyAutomaticallyRule(ScopeId == AllResources));
        CheckRule(new GranterMustManageAccessRule(granter));
        CheckRule(new GranterMustManageTheRoleRule(granter, role));
    }
}
