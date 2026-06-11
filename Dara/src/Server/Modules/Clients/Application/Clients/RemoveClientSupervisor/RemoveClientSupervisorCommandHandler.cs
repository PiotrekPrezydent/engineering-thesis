using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.RemoveClientSupervisor;

public class RemoveClientSupervisorCommandHandler : ICommandHandler<RemoveClientSupervisorCommand>
{
    readonly IClientRepository _clientRepository;
    public RemoveClientSupervisorCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task HandleAsync(RemoveClientSupervisorCommand command)
    {
        var clientId = new ClientId(command.ClientId);
        var supervisorId = new ClientId(command.SupervisorId);
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        client.RemoveSupervisor(supervisorId);
    }
}