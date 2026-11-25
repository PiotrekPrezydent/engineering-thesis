using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public interface IPluginRepository : IRepository
{
    Task AddAsync(Plugin plugin);
    
    Task<Plugin> GetByIdAsync(PluginId id);
    
    Task<IReadOnlyList<Plugin>> GetByOwnerIdAsync(PluginOwnerId ownerId);
    
    Task RemoveAsync(Plugin plugin);
}