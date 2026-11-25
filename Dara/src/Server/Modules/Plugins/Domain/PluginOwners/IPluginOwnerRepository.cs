using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public interface IPluginOwnerRepository : IRepository
{
    public Task AddAsync(PluginOwner pluginOwner);
    
    public Task<PluginOwner> GetByIdAsync(PluginOwnerId pluginOwnerId);
    
    // null when the owner is not projected yet, plugins can come before the new user event
    public Task<PluginOwner?> FindByIdAsync(PluginOwnerId pluginOwnerId);
}