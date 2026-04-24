using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Application.Exceptions;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Application.Clients.GetClient;

public class GetClientCommandHandler : IApplicationCommandHandler<GetClientCommand, GetClientCommandResult>
{
    private readonly IClientsRepository _clientsRepository;
    public GetClientCommandHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<GetClientCommandResult> HandleAsync(GetClientCommand command)
    {
        Domain.Clients.Client client;
        
        if (command is GetClientByConnectionIdCommand idCommand)
        {
            ClientConnectionId id = new ClientConnectionId(idCommand.ConnectionId);

            client = await _clientsRepository.GetByClientConnectionIdAsync(id);
        }
        else if (command is GetClientByGuidCommand guidCommand)
        {
            client = await _clientsRepository.GetByGuidAsync(guidCommand.ClientId);
        }
        else
        {
            throw new CommandTypeIsUnhandledException(this.GetType(),  command.GetType());
        }

        return new(client.Id, client.ClientConnectionId.Value, client.ClientConnectionIp.Value);
    }
}