using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public class NewUserCreatedIntegrationEventHandler : IIntegrationEventHandler<NewUserCreatedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public NewUserCreatedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }


    public async Task HandleAsync(NewUserCreatedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new CreateDefaultProfileCommand(integrationEvent.CreatedUserId));
    }
}