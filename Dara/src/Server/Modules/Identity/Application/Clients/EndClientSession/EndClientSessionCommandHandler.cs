using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.EndClientSession;

public class EndClientSessionCommandHandler : IHandler<EndClientSessionCommand>
{
    public EndClientSessionCommandHandler()
    {
    }
    
    public async Task HandleAsync(EndClientSessionCommand command)
    {
        return;
    }
}