using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

public interface IPluginConfigurationRepository : IRepository
{
    Task AddAsync(PluginConfiguration pluginConfiguration);
    
    Task<PluginConfiguration> GetByIdAsync(PluginConfigurationId id);
    
    Task RemoveAsync(PluginConfiguration pluginConfiguration);
}