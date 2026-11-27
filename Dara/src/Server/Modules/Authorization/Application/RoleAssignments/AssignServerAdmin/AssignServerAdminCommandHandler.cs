using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignServerAdmin;

// a second seed of the same subject changes nothing
public class AssignServerAdminCommandHandler : ICommandHandler<AssignServerAdminCommand>
{
    private readonly IRoleAssignmentRepository _assignmentRepository;
    private readonly IRoleRepository _roleRepository;

    public AssignServerAdminCommandHandler(IRoleAssignmentRepository assignmentRepository, IRoleRepository roleRepository)
    {
        _assignmentRepository = assignmentRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(AssignServerAdminCommand command)
    {
        var role = await _roleRepository.FindByCodeAsync(AuthorizationRoles.ServerAdmin.Code)
            ?? throw new InvalidOperationException($"role {AuthorizationRoles.ServerAdmin.Code} is not synchronized");

        if (await _assignmentRepository.FindAsync(command.SubjectId, role.Code, RoleAssignment.AllResources) is not null)
            return;

        await _assignmentRepository.AddAsync(RoleAssignment.Assign(command.SubjectId, role, RoleAssignment.AllResources));
    }
}
