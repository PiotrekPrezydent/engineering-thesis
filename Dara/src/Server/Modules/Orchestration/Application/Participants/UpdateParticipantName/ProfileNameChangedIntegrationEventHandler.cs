using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Profiles.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantName;

public class ProfileNameChangedIntegrationEventHandler : IIntegrationEventHandler<ProfileNameChangedIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public ProfileNameChangedIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(ProfileNameChangedIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new UpdateParticipantNameCommand(integrationEvent.ProfileId, integrationEvent.NewName));
    }
}