using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.ChangeClientName;

public class ClientChangedNameDomainEventHandler : IHandler<ClientChangedNameDomainEvent>
{
    public ClientChangedNameDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientChangedNameDomainEvent domainEvent)
    {
        return;
    }
}