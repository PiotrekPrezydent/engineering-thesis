using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments;

public interface IRoleAssignmentRepository : IRepository
{
    public Task<RoleAssignment?> FindAsync(Guid subjectId, string roleCode, Guid scopeId);

    // the assignments of the subject that hold on one resource: on it and on every resource (global ones)
    public Task<IReadOnlyList<RoleAssignment>> GetHeldOnAsync(Guid subjectId, Guid scopeId);

    public Task<IReadOnlyList<RoleAssignment>> GetWithRolesAsync(IReadOnlyCollection<string> roleCodes);

    // every assignment on the resource and every one the resource holds as a subject
    public Task<IReadOnlyList<RoleAssignment>> GetOnOrOfAsync(Guid id);

    public Task AddAsync(RoleAssignment assignment);

    public void Remove(RoleAssignment assignment);
}
