using Dara.BuildingBlocks.Application.Commands;
using Dara.Modules.Connections.Domain.Clients;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken
{
    public class ChangeClientAuthTokenCommandHandler : IModuleCommandHandler<ChangeClientAuthTokenCommand, ChangeClientAuthTokenCommandResult>
    {
        IClientRepository _clientRepository;
    
        public ChangeClientAuthTokenCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    
        public async Task<ChangeClientAuthTokenCommandResult> HandleAsync(ChangeClientAuthTokenCommand command)
        {
            ClientId id = new(command.ClientId);
            Client client = await _clientRepository.GetByIdAsync(id);

            ClientAuthToken newToken = new(command.NewAuthToken);
            client.ChangeAuthToken(newToken);
        
            await _clientRepository.SaveAsync(client);
        
            return new();
        }
    }
}