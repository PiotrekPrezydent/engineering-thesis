using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.AddClientSupervisor;

public class AddClientSupervisorCommandHandler : ICommandHandler<AddClientSupervisorCommand>
{
    private readonly IClientRepository _clientRepository;
    public AddClientSupervisorCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(AddClientSupervisorCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var superVisorId = new ClientId(command.SupervisorId);
        
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        client.AddSupervisor(superVisorId);
    }
}