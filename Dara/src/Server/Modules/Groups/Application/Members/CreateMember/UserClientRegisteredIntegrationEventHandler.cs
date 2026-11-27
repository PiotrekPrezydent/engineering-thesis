using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Groups.Application.Members.CreateMember;

public class UserClientRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserClientRegisteredIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public UserClientRegisteredIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(UserClientRegisteredIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new CreateMemberCommand(
            integrationEvent.ClientId,
            integrationEvent.UserId,
            integrationEvent.Name));
    }
}
