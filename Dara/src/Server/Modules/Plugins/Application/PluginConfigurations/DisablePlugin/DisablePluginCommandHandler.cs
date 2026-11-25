using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;

public class DisablePluginCommandHandler : ICommandHandler<DisablePluginCommand>
{
    private readonly IPluginConfigurationRepository _pluginConfigurationRepository;

    public DisablePluginCommandHandler(IPluginConfigurationRepository pluginConfigurationRepository)
    {
        _pluginConfigurationRepository = pluginConfigurationRepository;
    }

    public async Task HandleAsync(DisablePluginCommand command)
    {
        var configuration = await _pluginConfigurationRepository.GetByPluginIdAsync(new PluginId(command.PluginId));
        configuration.DisablePlugin(new PluginOwnerId(command.RequesterId));
    }
}
