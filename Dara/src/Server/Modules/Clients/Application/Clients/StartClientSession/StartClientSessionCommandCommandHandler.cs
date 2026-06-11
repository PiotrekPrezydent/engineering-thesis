using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.StartClientSession;

public class StartClientSessionCommandCommandHandler : ICommandHandler<StartClientSessionCommand>
{
    IClientRepository _clientRepository;
    
    public StartClientSessionCommandCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(StartClientSessionCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var client = Client.Create(clientId, command.ClientName);
        client.StartSession();
            
        await _clientRepository.Add(client);
    }
}