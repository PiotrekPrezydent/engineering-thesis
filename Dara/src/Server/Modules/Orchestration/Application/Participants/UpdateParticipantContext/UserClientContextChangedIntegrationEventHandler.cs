using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantContext;

public class UserClientContextChangedIntegrationEventHandler : IIntegrationEventHandler<UserClientContextChangedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public UserClientContextChangedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(UserClientContextChangedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new UpdateParticipantContextCommand(integrationEvent.ClientId, integrationEvent.Context));
    }
}
