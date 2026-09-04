using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.Plugins.AddPlugin;

public class AddPluginCommandHandler : ICommandHandler<AddPluginCommand>
{
    public AddPluginCommandHandler()
    {
    }
    
    public async Task HandleAsync(AddPluginCommand command)
    {
        return;
    }
}