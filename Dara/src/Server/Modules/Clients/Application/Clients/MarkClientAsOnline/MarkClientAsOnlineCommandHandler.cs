using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOnline;

public class MarkClientAsOnlineCommandHandler : ICommandHandler<MarkClientAsOnlineCommand>
{
    readonly IClientRepository _clientRepository;
    public MarkClientAsOnlineCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(MarkClientAsOnlineCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        client.MarkAsOnline();
    }
}