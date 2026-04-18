using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;

namespace Dara.Modules.RpcGateway.Application;

class SetConnectionLostCommandHandler : IApplicationCommandHandler<SetConnectionLostCommand, SetConnectionLostCommandResult>
{
    public SetConnectionLostCommandHandler()
    {
    }
    
    public async Task<SetConnectionLostCommandResult> HandleAsync(SetConnectionLostCommand command)
    {
        return new("123");
    }
}