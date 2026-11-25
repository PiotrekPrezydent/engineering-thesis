using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;

public class EnablePluginFunctionCommandHandler : ICommandHandler<EnablePluginFunctionCommand>
{
    private readonly IPluginConfigurationRepository _pluginConfigurationRepository;

    public EnablePluginFunctionCommandHandler(IPluginConfigurationRepository pluginConfigurationRepository)
    {
        _pluginConfigurationRepository = pluginConfigurationRepository;
    }

    public async Task HandleAsync(EnablePluginFunctionCommand command)
    {
        var configuration = await _pluginConfigurationRepository.GetByPluginIdAsync(new PluginId(command.PluginId));
        configuration.EnableFunction(new PluginOwnerId(command.RequesterId), new PluginFunctionId(command.FunctionId));
    }
}
