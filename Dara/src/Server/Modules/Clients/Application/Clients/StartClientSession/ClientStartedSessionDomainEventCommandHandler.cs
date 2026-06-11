using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Clients.Domain.Clients.Events;

namespace Dara.Server.Modules.Clients.Application.Clients.StartClientSession;

public class ClientStartedSessionDomainEventCommandHandler : IDomainEventHandler<ClientStartedSessionDomainEvent>
{
    public ClientStartedSessionDomainEventCommandHandler()
    {
    }
    
    public async Task HandleAsync(ClientStartedSessionDomainEvent domainEvent)
    {
        return;
    }
}