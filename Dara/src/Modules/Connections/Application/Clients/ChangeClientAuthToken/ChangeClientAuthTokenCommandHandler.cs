using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken
{
    public class ChangeClientAuthTokenCommandHandler : IHandler<ChangeClientAuthTokenCommand>
    {
        IClientRepository _clientRepository;
    
        public ChangeClientAuthTokenCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    
        public async Task HandleAsync(ChangeClientAuthTokenCommand command)
        {
            ClientId id = new(command.ClientId);
            Client client = await _clientRepository.GetByIdAsync(id);

            ClientAuthToken newToken = new(command.NewAuthToken);
            client.ChangeAuthToken(newToken);
        
            await _clientRepository.SaveAsync(client);
        }
    }
}