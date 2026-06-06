using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.RemoveClientPlugin;

public class RemoveClientPluginCommandHandler : IHandler<RemoveClientPluginCommand>
{
    public RemoveClientPluginCommandHandler()
    {
    }
    
    public async Task HandleAsync(RemoveClientPluginCommand command)
    {
        return;
    }
}