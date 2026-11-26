using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Profiles.Integration;

namespace Dara.Server.Modules.Groups.Application.Members.UpdateMemberName;

public class ProfileNameChangedIntegrationEventHandler : IIntegrationEventHandler<ProfileNameChangedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public ProfileNameChangedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(ProfileNameChangedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new UpdateMemberNameCommand(integrationEvent.ProfileId, integrationEvent.NewName));
    }
}