using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOffline;

public class MarkClientAsOfflineCommandHandler : ICommandHandler<MarkClientAsOfflineCommand>
{
    readonly IClientRepository _clientRepository;
    public MarkClientAsOfflineCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(MarkClientAsOfflineCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        client.MarkAsOffline();
    }
}