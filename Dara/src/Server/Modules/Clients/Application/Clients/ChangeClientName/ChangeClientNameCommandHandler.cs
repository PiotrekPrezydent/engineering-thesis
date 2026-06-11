using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.ChangeClientName;

public class ChangeClientNameCommandHandler : ICommandHandler<ChangeClientNameCommand>
{
    readonly IClientRepository _clientRepository;
    public ChangeClientNameCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(ChangeClientNameCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        client.ChangeName(command.NewName);
    }
}