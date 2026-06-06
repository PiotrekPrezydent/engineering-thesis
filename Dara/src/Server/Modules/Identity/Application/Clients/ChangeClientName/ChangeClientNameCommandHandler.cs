using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.ChangeClientName;

public class ChangeClientNameCommandHandler : IHandler<ChangeClientNameCommand>
{
    public ChangeClientNameCommandHandler()
    {
    }
    
    public async Task HandleAsync(ChangeClientNameCommand command)
    {
        return;
    }
}