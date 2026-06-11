using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.DeleteClient;

public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand>
{
    readonly IClientRepository _clientRepository;
    public DeleteClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(DeleteClientCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        client.Delete();
        await _clientRepository.DeleteAsync(client); // check if this will prevent domain event to be called
    }
}