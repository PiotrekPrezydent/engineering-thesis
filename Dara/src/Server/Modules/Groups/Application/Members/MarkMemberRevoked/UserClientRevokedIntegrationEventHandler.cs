using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Groups.Application.Members.MarkMemberRevoked;

public class UserClientRevokedIntegrationEventHandler : IIntegrationEventHandler<UserClientRevokedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public UserClientRevokedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(UserClientRevokedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new MarkMemberRevokedCommand(integrationEvent.ClientId));
    }
}
