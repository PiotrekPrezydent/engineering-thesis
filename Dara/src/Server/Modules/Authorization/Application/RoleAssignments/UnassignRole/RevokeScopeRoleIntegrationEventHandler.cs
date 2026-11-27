using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration.Authorization;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.UnassignRole;

public class RevokeScopeRoleIntegrationEventHandler : IIntegrationEventHandler<RevokeScopeRoleIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public RevokeScopeRoleIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(RevokeScopeRoleIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new UnassignRoleCommand(integrationEvent.SubjectId, integrationEvent.RoleCode,
            integrationEvent.ScopeId));
    }
}
