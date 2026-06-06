using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.RemoveClientSupervisor;

public class RemoveClientSupervisorCommandHandler : IHandler<RemoveClientSupervisorCommand>
{
    public RemoveClientSupervisorCommandHandler()
    {
    }
    
    public async Task HandleAsync(RemoveClientSupervisorCommand command)
    {
        return;
    }
}