using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients.Events;

namespace Dara.Modules.Communication.Application.Clients.ChangeClientName;

public class ClientNameChangedEventHandler : IDomainEventHandler<ClientNameChangedDomainEvent>
{
    public ClientNameChangedEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientNameChangedDomainEvent domainEvent)
    {
    }
}