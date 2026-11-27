using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Infrastructure.RoleAssignments;

public class RoleAssignmentRepository : IRoleAssignmentRepository
{
    private readonly AuthorizationContext _context;

    public RoleAssignmentRepository(AuthorizationContext context)
    {
        _context = context;
    }

    public async Task<RoleAssignment?> FindAsync(Guid subjectId, string roleCode, Guid scopeId)
    {
        return await _context.RoleAssignments.FirstOrDefaultAsync(a =>
            a.SubjectId == subjectId && a.RoleCode == roleCode && a.ScopeId == scopeId);
    }

    public async Task<IReadOnlyList<RoleAssignment>> GetHeldOnAsync(Guid subjectId, Guid scopeId)
    {
        var all = RoleAssignment.AllResources;
        return await _context.RoleAssignments
            .Where(a => a.SubjectId == subjectId && (a.ScopeId == scopeId || a.ScopeId == all))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<RoleAssignment>> GetWithRolesAsync(IReadOnlyCollection<string> roleCodes)
    {
        var codes = roleCodes.ToArray();
        return await _context.RoleAssignments.Where(a => codes.Contains(a.RoleCode)).ToListAsync();
    }

    public async Task<IReadOnlyList<RoleAssignment>> GetOnOrOfAsync(Guid id)
    {
        return await _context.RoleAssignments.Where(a => a.ScopeId == id || a.SubjectId == id).ToListAsync();
    }

    public async Task AddAsync(RoleAssignment assignment)
    {
        await _context.RoleAssignments.AddAsync(assignment);
    }

    public void Remove(RoleAssignment assignment)
    {
        _context.RoleAssignments.Remove(assignment);
    }
}
