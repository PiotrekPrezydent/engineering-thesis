using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;

namespace Dara.Modules.RpcGateway.Application;

class SetConnectionEstablishedCommandHandler : IApplicationCommandHandler<SetConnectionEstablishedCommand, SetConnectionEstablishedCommandResult>
{
    public SetConnectionEstablishedCommandHandler()
    {
    }
    
    public async Task<SetConnectionEstablishedCommandResult> HandleAsync(SetConnectionEstablishedCommand command)
    {
        return new();
    }
}