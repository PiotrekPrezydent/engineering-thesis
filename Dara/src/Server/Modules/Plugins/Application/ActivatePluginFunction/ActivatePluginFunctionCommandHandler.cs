using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.ActivatePluginFunction;

public class ActivatePluginFunctionCommandHandler : ICommandHandler<ActivatePluginFunctionCommand>
{
    private readonly IPluginOwnerRepository _pluginOwnerRepository;

    public ActivatePluginFunctionCommandHandler(IPluginOwnerRepository pluginOwnerRepository)
    {
        _pluginOwnerRepository = pluginOwnerRepository;
    }

    public async Task HandleAsync(ActivatePluginFunctionCommand command)
    {
        var ownerId = new PluginOwnerId(command.PluginOwnerId);
        var pluginId = new PluginId(command.PluginId);
        var pluginFunctionId = new PluginFunctionId(command.PluginFunctionId);

        var owner = await _pluginOwnerRepository.GetByIdAsync(ownerId);
        owner.ActivateFunction(pluginId, pluginFunctionId);
    }
}