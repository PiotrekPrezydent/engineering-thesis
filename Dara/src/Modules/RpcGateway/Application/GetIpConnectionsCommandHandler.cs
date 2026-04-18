using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;

namespace Dara.Modules.RpcGateway.Application;

public class GetIpConnectionsCommandHandler : IApplicationCommandHandler<GetIpConnectionsCommand, GetIpConnectionsCommandResult>
{
    private readonly IConnectionRepository _connectionRepository;
    
    public GetIpConnectionsCommandHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }
    
    public async Task<GetIpConnectionsCommandResult> HandleAsync(GetIpConnectionsCommand command)
    {
        ConnectionIp ip = new(command.Ip); //create as object for validation check if added later

        var result = await _connectionRepository.GetAllWithIpAsync(ip);
        
        return new(result.Select(e=>e.ConnectionId.Value));
    }
}