using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.Clients.EndClientSession;

public class ClientEndedSessionDomainEventHandler : IHandler<ClientEndedSessionDomainEvent>
{
    public ClientEndedSessionDomainEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientEndedSessionDomainEvent domainEvent)
    {
        return;
    }
}