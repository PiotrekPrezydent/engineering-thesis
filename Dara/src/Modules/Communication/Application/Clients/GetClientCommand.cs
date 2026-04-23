using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Application.Clients;

public abstract record GetClientCommand() : IApplicationCommand;

public record GetClientByConnectionIdCommand(string ConnectionId) : GetClientCommand;

public record GetClientByGuidCommand(Guid ClientId) : GetClientCommand;

public record GetClientCommandResult(Guid ClientId, string ConnectionId, string ConnectionIp) : IApplicationCommandResult;

public class GetClientCommandHandler : IApplicationCommandHandler<GetClientCommand, GetClientCommandResult>
{
    private readonly IClientsRepository _clientsRepository;
    public GetClientCommandHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<GetClientCommandResult> HandleAsync(GetClientCommand command)
    {
        if (command is GetClientByConnectionIdCommand)
        {
            var com = command as GetClientByConnectionIdCommand;
            
            ClientConnectionId id = new ClientConnectionId(com.ConnectionId);

            var client = await _clientsRepository.GetByClientConnectionIdAsync(id);
            
            return new(client.Id, client.ClientConnectionId.Value, client.ClientConnectionIp.Value);
        }

        return null;
    }
}