using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Application.Clients;

public record CreateClientCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;

public record CreateClientCommandResult(Guid CreatedClientId) : IApplicationCommandResult;

public class CreateClientCommandHandler : IApplicationCommandHandler<CreateClientCommand, CreateClientCommandResult>
{
    private readonly IClientsRepository _clientsRepository;
    public CreateClientCommandHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<CreateClientCommandResult> HandleAsync(CreateClientCommand command)
    {
        ClientConnectionId id = new(command.ConnectionId);
        ClientConnectionIp ip = new(command.ConnectionIp);
        
        Client client = new(id, ip);
        
        await _clientsRepository.AddAsync(client);
        
        return new(client.Id);
    }
}