using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.CreateClient;

public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand>
{
    readonly IClientRepository _clientRepository;
    public CreateClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(CreateClientCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var client = Client.Create(clientId, command.ClientName);
        
        await _clientRepository.AddAsync(client);
    }
}