using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.UnassignRole;

// a repeated event, or one for an assignment that is gone, changes nothing
public class UnassignRoleCommandHandler : ICommandHandler<UnassignRoleCommand>
{
    private readonly IRoleAssignmentRepository _assignmentRepository;

    public UnassignRoleCommandHandler(IRoleAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task HandleAsync(UnassignRoleCommand command)
    {
        var assignment = await _assignmentRepository.FindAsync(command.SubjectId, command.RoleCode, command.ScopeId);
        if (assignment is not null)
            _assignmentRepository.Remove(assignment);
    }
}
