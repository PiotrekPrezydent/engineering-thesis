using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;

//td: change add Handler to file name, add module contracts using

namespace Dara.Modules.RpcGateway.Application;

class GetIpConnectionsCommandHandler : IApplicationCommandHandler<GetIpConnectionsCommand, GetIpConnectionsCommandResult>
{
    public GetIpConnectionsCommandHandler()
    {
    }
    
    public async Task<GetIpConnectionsCommandResult> HandleAsync(GetIpConnectionsCommand command)
    {
        List<string> result = new();
        
        return new(result);
    }
}