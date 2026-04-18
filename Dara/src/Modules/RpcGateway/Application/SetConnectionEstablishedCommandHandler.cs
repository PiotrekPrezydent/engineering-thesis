using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;

namespace Dara.Modules.RpcGateway.Application;

public class SetConnectionEstablishedCommandHandler : IApplicationCommandHandler<SetConnectionEstablishedCommand, SetConnectionEstablishedCommandResult>
{
    private readonly IConnectionRepository _connectionRepository;
    
    public SetConnectionEstablishedCommandHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }
    
    public async Task<SetConnectionEstablishedCommandResult> HandleAsync(SetConnectionEstablishedCommand command)
    {
        ConnectionId id = new(command.ConnectionId);
        ConnectionIp ip = new(command.ConnectionIp);
        
        Connection connection = new(id, ip);
        
        connection.BecomeEstablished();

        await _connectionRepository.AddAsync(connection);
        
        return new();
    }
}