using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.RemoveClientSupervisor;

public class ClientRemovedSupervisorDomainEventHandler : IHandler<ClientRemovedSupervisorDomainEvent>
{
    public ClientRemovedSupervisorDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientRemovedSupervisorDomainEvent domainEvent)
    {
        return;
    }
}