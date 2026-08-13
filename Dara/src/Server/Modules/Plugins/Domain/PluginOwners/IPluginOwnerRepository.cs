using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public interface IPluginOwnerRepository : IRepository
{
    public Task AddAsync(PluginOwner pluginOwner);
    
    public Task<PluginOwner> GetByIdAsync(PluginOwnerId pluginOwnerId);
}