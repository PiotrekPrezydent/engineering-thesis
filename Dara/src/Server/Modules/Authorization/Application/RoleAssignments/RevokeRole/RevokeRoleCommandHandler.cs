using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.RevokeRole;

// every assignment is checked before any is removed, so a broken rule on one companion revokes nothing
public class RevokeRoleCommandHandler : ICommandHandler<RevokeRoleCommand>
{
    private readonly IRoleAssignmentRepository _assignmentRepository;
    private readonly IRoleRepository _roleRepository;

    public RevokeRoleCommandHandler(IRoleAssignmentRepository assignmentRepository, IRoleRepository roleRepository)
    {
        _assignmentRepository = assignmentRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(RevokeRoleCommand command)
    {
        var revokes = await Companions.ResolveAsync(_roleRepository, command.RoleCode, command.ScopeId, command.Companions);

        var assignments = new List<RoleAssignment>();
        foreach (var (role, scopeId) in revokes)
        {
            var assignment = await _assignmentRepository.FindAsync(command.SubjectId, role.Code, scopeId);
            Companions.Check(new RoleMustBeAssignedRule(assignment));

            var granter = await SubjectAccess.ReadGranterAsync(_assignmentRepository, _roleRepository, command.SenderId, role.ScopeCode, scopeId);
            assignment!.Revoke(granter, role);
            assignments.Add(assignment);
        }

        foreach (var assignment in assignments)
            _assignmentRepository.Remove(assignment);
    }
}
