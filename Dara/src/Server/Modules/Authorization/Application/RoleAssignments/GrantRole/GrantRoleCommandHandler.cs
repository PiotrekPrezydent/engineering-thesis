using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.GrantRole;

// every assignment is checked before any is added, so a broken rule on one companion grants nothing
public class GrantRoleCommandHandler : ICommandHandler<GrantRoleCommand>
{
    private readonly IRoleAssignmentRepository _assignmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleAssignmentChecker _checker;

    public GrantRoleCommandHandler(IRoleAssignmentRepository assignmentRepository, IRoleRepository roleRepository,
        IRoleAssignmentChecker checker)
    {
        _assignmentRepository = assignmentRepository;
        _roleRepository = roleRepository;
        _checker = checker;
    }

    public async Task HandleAsync(GrantRoleCommand command)
    {
        var grants = await Companions.ResolveAsync(_roleRepository, command.RoleCode, command.ScopeId, command.Companions);

        var assignments = new List<RoleAssignment>();
        foreach (var (role, scopeId) in grants)
        {
            var granter = await SubjectAccess.ReadGranterAsync(_assignmentRepository, _roleRepository, command.SenderId, role.ScopeCode, scopeId);
            assignments.Add(RoleAssignment.Grant(command.SubjectId, role, scopeId, granter, _checker));
        }

        foreach (var assignment in assignments)
            await _assignmentRepository.AddAsync(assignment);
    }
}
