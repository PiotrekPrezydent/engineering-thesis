using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public class NewUserCreatedIntegrationEventHandler : IIntegrationEventHandler<NewUserCreatedIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public NewUserCreatedIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }


    public async Task HandleAsync(NewUserCreatedIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new CreateDefaultProfileCommand(integrationEvent.CreatedUserId));
    }
}