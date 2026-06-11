using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Clients.Domain.Clients.Events;

namespace Dara.Server.Modules.Clients.Application.Clients.ChangeClientName;
public class ClientChangedNameDomainEventCommandHandler : IDomainEventHandler<ClientChangedNameDomainEvent>
{
    public ClientChangedNameDomainEventCommandHandler()
    {
    }
    
    public async Task HandleAsync(ClientChangedNameDomainEvent domainEvent)
    {
        return;
    }
}