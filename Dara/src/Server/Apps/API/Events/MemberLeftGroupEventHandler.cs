using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Apps.API.Events;

public class MemberLeftGroupEventHandler : IIntegrationEventHandler<MemberLeftGroupIntegrationEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public MemberLeftGroupEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(MemberLeftGroupIntegrationEvent integrationEvent)
    {
        throw new NotImplementedException();
    }
}