using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public class NewClientCreatedIntegrationEventHandler : IIntegrationEventHandler<NewClientCreatedIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public NewClientCreatedIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }


    public async Task HandleAsync(NewClientCreatedIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new CreateProfileCommand(integrationEvent.CreatedClientId,"DEFAULT-NAME"));
    }
}