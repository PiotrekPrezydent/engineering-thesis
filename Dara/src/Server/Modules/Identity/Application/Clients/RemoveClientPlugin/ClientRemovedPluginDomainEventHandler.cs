using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.RemoveClientPlugin;

public class ClientRemovedPluginDomainEventHandler : IHandler<ClientRemovedPluginDomainEvent>
{
    public ClientRemovedPluginDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientRemovedPluginDomainEvent domainEvent)
    {
        return;
    }
}