using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.Plugins.Events;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations;

// runs before the plugin is saved, so it takes the function ids from the event instead of reading the plugin
public class PluginAddedDomainEventHandler : IDomainEventHandler<PluginAddedDomainEvent>
{
    private readonly IPluginConfigurationRepository _pluginConfigurationRepository;

    public PluginAddedDomainEventHandler(IPluginConfigurationRepository pluginConfigurationRepository)
    {
        _pluginConfigurationRepository = pluginConfigurationRepository;
    }

    public async Task HandleAsync(PluginAddedDomainEvent domainEvent)
    {
        var configuration = PluginConfiguration.Create(domainEvent.PluginOwnerId, domainEvent.PluginId, domainEvent.FunctionIds);
        await _pluginConfigurationRepository.AddAsync(configuration);
    }
}