using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.StartClientSession;

public class ClientStartedSessionDomainEventHandler : IHandler<ClientStartedSessionDomainEvent>
{
    public ClientStartedSessionDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientStartedSessionDomainEvent domainEvent)
    {
        return;
    }
}