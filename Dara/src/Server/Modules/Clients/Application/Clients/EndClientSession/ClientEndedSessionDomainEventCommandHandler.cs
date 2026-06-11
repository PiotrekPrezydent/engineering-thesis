using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients.Events;

namespace Dara.Server.Modules.Clients.Application.Clients.EndClientSession;

public class ClientEndedSessionDomainEventCommandHandler : ICommandHandler<ClientEndedSessionDomainEvent>
{
    public ClientEndedSessionDomainEventCommandHandler()
    {
    }
    
    public async Task HandleAsync(ClientEndedSessionDomainEvent domainEvent)
    {
        return;
    }
}