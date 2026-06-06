using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.AddClientPlugin;

public class ClientAddedPluginDomainEventHandler : IHandler<ClientAddedPluginDomainEvent>
{
    public ClientAddedPluginDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientAddedPluginDomainEvent domainEvent)
    {
        return;
    }
}