using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.ChangeClientName;

public class ChangeClientNameCommandCommandHandler : ICommandHandler<ChangeClientNameCommand>
{
    IClientRepository _clientRepository { get; }
    public ChangeClientNameCommandCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(ChangeClientNameCommand command)
    {
        var clientId = new ClientId(command.ClientId);

        var client = await _clientRepository.GetByClientIdAsync(clientId);
        
        client.ChangeName(command.NewName);
        
        await _clientRepository.SaveAsync(client);
    }
}