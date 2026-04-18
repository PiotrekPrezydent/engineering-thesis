using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;

namespace Dara.Modules.RpcGateway.Application;

public class SetConnectionLostCommandHandler : IApplicationCommandHandler<SetConnectionLostCommand, SetConnectionLostCommandResult>
{
    private readonly IConnectionRepository _connectionRepository;
    
    public SetConnectionLostCommandHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }

    public async Task<SetConnectionLostCommandResult> HandleAsync(SetConnectionLostCommand command)
    {
        ConnectionId id = new ConnectionId(command.ConnectionId);

        var connetion = await _connectionRepository.GetByConnectionIdAsync(id);
        
        connetion.BecomeLost();
        
        await _connectionRepository.RemoveAsync(connetion);
        
        return new(command.ConnectionId);
    }
}