using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantName;

public class UserClientRenamedIntegrationEventHandler : IIntegrationEventHandler<UserClientRenamedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public UserClientRenamedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(UserClientRenamedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new UpdateParticipantNameCommand(integrationEvent.ClientId, integrationEvent.NewName));
    }
}
