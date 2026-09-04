using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public interface IPluginRepository : IRepository
{
    Task AddAsync(Plugin plugin);
    
    Task<Plugin> GetByIdAsync(PluginId id);
    
    Task RemoveAsync(Plugin plugin);
}