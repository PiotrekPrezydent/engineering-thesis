using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Application.Clients.CreateClient;

public class CreateClientCommandHandler : IApplicationCommandHandler<CreateClientCommand, CreateClientCommandResult>
{
    IConnectionRepository _connectionRepository;
    IClientRepository _clientRepository;
    
    public CreateClientCommandHandler(IConnectionRepository connectionRepository, IClientRepository clientRepository)
    {
        _connectionRepository = connectionRepository;
        _clientRepository = clientRepository;
    }
    
    public async Task<CreateClientCommandResult> HandleAsync(CreateClientCommand command)
    {
        ConnectionId id = new(command.ConnectionId);
        
        Connection connection = await _connectionRepository.GetByIdAsync(id);

        ClientName name = new ClientName(command.ClientName);
        ClientAuthToken token = new ClientAuthToken(command.ClientAuthToken);
        
        Client client = connection.CreateClient(name, token);
        
        await _clientRepository.AddAsync(client);
        
        return new(client.Id.Value);
    }
}