using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Groups.Application;

public class NewUserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    public async Task HandleAsync(UserCreatedIntegrationEvent integrationEvent)
    {
        Console.WriteLine($"CALLED ::::: NewUserCreatedIntegrationEvent integrationEvent: {integrationEvent}");
    }
}