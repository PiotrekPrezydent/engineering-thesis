using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;

public class DisablePluginFunctionCommandHandler : ICommandHandler<DisablePluginFunctionCommand>
{
    private readonly IPluginConfigurationRepository _pluginConfigurationRepository;

    public DisablePluginFunctionCommandHandler(IPluginConfigurationRepository pluginConfigurationRepository)
    {
        _pluginConfigurationRepository = pluginConfigurationRepository;
    }

    public async Task HandleAsync(DisablePluginFunctionCommand command)
    {
        var configuration = await _pluginConfigurationRepository.GetByPluginIdAsync(new PluginId(command.PluginId));
        configuration.DisableFunction(new PluginOwnerId(command.RequesterId), new PluginFunctionId(command.FunctionId));
    }
}
