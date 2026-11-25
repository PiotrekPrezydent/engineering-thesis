using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

public interface IPluginConfigurationRepository : IRepository
{
    Task AddAsync(PluginConfiguration pluginConfiguration);
    
    Task<PluginConfiguration> GetByIdAsync(PluginConfigurationId id);
    
    Task<PluginConfiguration> GetByPluginIdAsync(PluginId pluginId);

    Task RemoveAsync(PluginConfiguration pluginConfiguration);
}