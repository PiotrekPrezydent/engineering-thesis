using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration.Authorization;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.RemoveScope;

public class ScopeRemovedIntegrationEventHandler : IIntegrationEventHandler<ScopeRemovedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public ScopeRemovedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(ScopeRemovedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new RemoveScopeCommand(integrationEvent.ScopeCode, integrationEvent.ScopeId));
    }
}
