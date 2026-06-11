using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients.Events;

namespace Dara.Server.Modules.Clients.Application.Clients.StartClientSession;

public class ClientStartedSessionDomainEventCommandHandler : ICommandHandler<ClientStartedSessionDomainEvent>
{
    public ClientStartedSessionDomainEventCommandHandler()
    {
    }
    
    public async Task HandleAsync(ClientStartedSessionDomainEvent domainEvent)
    {
        return;
    }
}