using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.RemoveScope;

// every assignment on the resource and every one it held as a subject (a revoked client) disappears; ids are unique
// across the system, so the id alone is enough
public class RemoveScopeCommandHandler : ICommandHandler<RemoveScopeCommand>
{
    private readonly IRoleAssignmentRepository _assignmentRepository;

    public RemoveScopeCommandHandler(IRoleAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task HandleAsync(RemoveScopeCommand command)
    {
        foreach (var assignment in await _assignmentRepository.GetOnOrOfAsync(command.ScopeId))
            _assignmentRepository.Remove(assignment);
    }
}
