using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.AddClientSupervisor;

public class ClientAddedSupervisorDomainEventHandler : IHandler<ClientAddedSupervisorDomainEvent>
{
    public ClientAddedSupervisorDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientAddedSupervisorDomainEvent domainEvent)
    {
        return;
    }
}