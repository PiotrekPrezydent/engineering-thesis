using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.DeactivatePluginFunction;

public class DeactivatePluginFunctionCommandHandler : ICommandHandler<DeactivatePluginFunctionCommand>
{
    private readonly IPluginOwnerRepository _repository;

    public DeactivatePluginFunctionCommandHandler(IPluginOwnerRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeactivatePluginFunctionCommand command)
    {
        var ownerId = new PluginOwnerId(command.PluginOwnerId);
        var pluginId = new PluginId(command.PluginId);
        var pluginFunctionId = new PluginFunctionId(command.PluginFunctionId);

        var owner = await _repository.GetByIdAsync(ownerId);
        owner.DeactivateFunction(pluginId, pluginFunctionId);
    }
}