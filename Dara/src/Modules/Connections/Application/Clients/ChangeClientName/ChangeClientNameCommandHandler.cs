using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientName
{
    public class ChangeClientNameCommandHandler : IHandler<ChangeClientNameCommand>
    {
        IClientRepository _clientRepository;
        public ChangeClientNameCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    
        public async Task HandleAsync(ChangeClientNameCommand command)
        {
            ClientId id = new(command.ClientId);
            Client client = await _clientRepository.GetByIdAsync(id);
        
            ClientName newName = new(command.NewName);
            client.ChangeName(newName);
        
            await _clientRepository.SaveAsync(client);
        }
    }
}