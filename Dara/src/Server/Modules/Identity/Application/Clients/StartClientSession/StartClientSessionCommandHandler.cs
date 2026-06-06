using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.StartClientSession;

public class StartClientSessionCommandHandler : IHandler<StartClientSessionCommand>
{
    public StartClientSessionCommandHandler()
    {
    }
    
    public async Task HandleAsync(StartClientSessionCommand command)
    {
        return;
    }
}