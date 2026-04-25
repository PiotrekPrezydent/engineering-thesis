using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientName
{
    public class ChangeClientNameCommandHandler : IApplicationCommandHandler<ChangeClientNameCommand, ChangeClientNameCommandResult>
    {
        IClientRepository _clientRepository;
        public ChangeClientNameCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    
        public async Task<ChangeClientNameCommandResult> HandleAsync(ChangeClientNameCommand command)
        {
            ClientId id = new(command.ClientId);
            Client client = await _clientRepository.GetByIdAsync(id);
        
            ClientName newName = new(command.NewName);
            client.ChangeName(newName);
        
            await _clientRepository.SaveAsync(client);
        
            return new();
        }
    }
}