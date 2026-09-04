using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Domain.Plugins.Events;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.CreatePluginConfiguration;

public class PluginAddedDomainEventHandler : IDomainEventHandler<PluginAddedDomainEvent>
{
    private readonly IReadModel _readModel;
    private readonly IPluginConfigurationRepository _pluginConfigurationRepository;

    public PluginAddedDomainEventHandler(IReadModel readModel, IPluginConfigurationRepository pluginConfigurationRepository)
    {
        _readModel = readModel;
        _pluginConfigurationRepository = pluginConfigurationRepository;
    }

    public async Task HandleAsync(PluginAddedDomainEvent domainEvent)
    {
        var plugin = await _readModel.Query<Plugin>().FirstAsync(e => e.Id.Match(domainEvent.PluginId));

        var configuration = PluginConfiguration.Create(plugin.Id, plugin.Functions.Select(e => e.Id).ToList());
        await _pluginConfigurationRepository.AddAsync(configuration);
    }
}