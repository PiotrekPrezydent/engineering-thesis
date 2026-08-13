using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.RemovePlugin;

public class RemovePluginCommandHandler : ICommandHandler<RemovePluginCommand>
{
    private readonly IPluginOwnerRepository _pluginOwnerRepository;

    public RemovePluginCommandHandler(IPluginOwnerRepository pluginOwnerRepository)
    {
        _pluginOwnerRepository = pluginOwnerRepository;
    }

    public async Task HandleAsync(RemovePluginCommand command)
    {
        var ownerId = new PluginOwnerId(command.OwnerId);
        var owner = await _pluginOwnerRepository.GetByIdAsync(ownerId);
        owner.RemovePlugin(new PluginId(command.PluginId));
    }
}