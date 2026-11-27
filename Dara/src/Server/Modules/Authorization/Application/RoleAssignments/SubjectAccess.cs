using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments;

// what one subject holds: only its own assignments count, whoever it is (a client does not get the roles of its user)
internal static class SubjectAccess
{
    // the assignments of the subject, optionally only those that hold on one resource (on it or global), with the
    // permission codes of their roles
    public static async Task<List<(RoleAssignment Assignment, IReadOnlyList<string> PermissionCodes)>> ReadAsync(
        IReadModel readModel, Guid subjectId, Guid? scopeId = null)
    {
        var query = readModel.Query<RoleAssignment>().Where(a => a.SubjectId == subjectId);
        if (scopeId is { } resourceId)
        {
            var all = RoleAssignment.AllResources;
            query = query.Where(a => a.ScopeId == resourceId || a.ScopeId == all);
        }

        var assignments = await query.ToListAsync();
        var roleCodes = assignments.Select(a => a.RoleCode).Distinct().ToList();
        var roles = await readModel.Query<Role>().Where(r => roleCodes.Contains(r.Code)).ToListAsync();

        return assignments
            .Select(a => (a, roles.FirstOrDefault(r => r.Code == a.RoleCode && r.ScopeCode == a.ScopeCode)?.PermissionCodes ?? (IReadOnlyList<string>)[]))
            .ToList();
    }

    // what the sender holds on the resource when it grants or revokes a role of the scope; the administrator of the
    // server is the one with Authorization.ServerAdmin on every resource
    public static async Task<GranterAccess> ReadGranterAsync(IRoleAssignmentRepository assignments, IRoleRepository roles,
        Guid senderId, string scopeCode, Guid scopeId)
    {
        var held = await assignments.GetHeldOnAsync(senderId, scopeId);
        var allRoles = await roles.GetAllAsync();

        var inScope = held.Where(a => a.ScopeCode == scopeCode).Select(a => a.RoleCode).ToHashSet();
        var scopeRoles = allRoles.Where(r => r.ScopeCode == scopeCode && inScope.Contains(r.Code)).ToList();

        var global = held.Where(a => a.ScopeId == RoleAssignment.AllResources).Select(a => a.RoleCode).ToHashSet();
        var isServerAdmin = allRoles.Any(r => global.Contains(r.Code) && r.PermissionCodes.Contains(AuthorizationPermissions.Administer.Code));

        return new GranterAccess(
            scopeRoles.SelectMany(r => r.PermissionCodes).ToHashSet(),
            scopeRoles.SelectMany(r => r.ManageableRoleCodes).ToHashSet(),
            ScopeDefinition.ManageAccessPermissionCodeOf(scopeCode),
            isServerAdmin);
    }
}
