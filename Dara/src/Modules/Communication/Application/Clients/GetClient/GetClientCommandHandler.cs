using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Application.Clients.GetClient;

public class GetClientCommandHandler : IApplicationCommandHandler<GetClientCommand, GetClientCommandResult>
{
    private IConnectionRepository _connectionRepository;
    
    public GetClientCommandHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }
    
    public async Task<GetClientCommandResult> HandleAsync(GetClientCommand command)
    {
        ConnectionId id = new(command.ConnectionId);

        Connection connection = await _connectionRepository.GetByIdAsync(id);
        if (connection.TryGetClient(out Client client))
        {
            return new(client.Id.Value);
        }
        else
        {
            throw new InvalidOperationException($"Could not find client with connection id {command.ConnectionId}");
        }
    }
}