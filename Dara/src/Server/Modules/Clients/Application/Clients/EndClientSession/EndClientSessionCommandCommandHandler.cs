using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.EndClientSession;

public class EndClientSessionCommandCommandHandler : ICommandHandler<EndClientSessionCommand>
{
    IClientRepository _clientRepository;
    public EndClientSessionCommandCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(EndClientSessionCommand command)
    {
        var clientId = new ClientId(command.ClientId);

        var client = await _clientRepository.GetByClientIdAsync(clientId);
        
        client.EndSession();
        
        await _clientRepository.SaveAsync(client);
    }
}