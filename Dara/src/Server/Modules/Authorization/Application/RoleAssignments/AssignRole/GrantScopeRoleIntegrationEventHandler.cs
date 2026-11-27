using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration.Authorization;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignRole;

public class GrantScopeRoleIntegrationEventHandler : IIntegrationEventHandler<GrantScopeRoleIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public GrantScopeRoleIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(GrantScopeRoleIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new AssignRoleCommand(integrationEvent.SubjectId, integrationEvent.RoleCode,
            integrationEvent.ScopeCode, integrationEvent.ScopeId));
    }
}
