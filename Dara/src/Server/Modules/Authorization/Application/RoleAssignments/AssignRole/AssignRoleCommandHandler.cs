using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignRole;

// a repeated event changes nothing; a role no module declares (or of another scope) is an error of the module that
// sent the event, so the command fails and is retried and parked like any failing internal command
public class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand>
{
    private readonly IRoleAssignmentRepository _assignmentRepository;
    private readonly IRoleRepository _roleRepository;

    public AssignRoleCommandHandler(IRoleAssignmentRepository assignmentRepository, IRoleRepository roleRepository)
    {
        _assignmentRepository = assignmentRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(AssignRoleCommand command)
    {
        var role = await _roleRepository.FindByCodeAsync(command.RoleCode);
        if (role is null || role.ScopeCode != command.ScopeCode)
            throw new InvalidOperationException($"no module declares role {command.RoleCode} in scope {command.ScopeCode}");

        if (await _assignmentRepository.FindAsync(command.SubjectId, command.RoleCode, command.ScopeId) is not null)
            return;

        await _assignmentRepository.AddAsync(RoleAssignment.Assign(command.SubjectId, role, command.ScopeId));
    }
}
