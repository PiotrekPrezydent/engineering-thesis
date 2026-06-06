using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.AddClientPlugin;

public class AddClientPluginCommandHandler : IHandler<AddClientPluginCommand>
{
    public AddClientPluginCommandHandler()
    {
    }
    
    public async Task HandleAsync(AddClientPluginCommand command)
    {
        return;
    }
}