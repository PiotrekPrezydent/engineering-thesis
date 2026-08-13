using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners;

public class PluginOwnerRepository : IPluginOwnerRepository
{
    private readonly PluginsContext _context;

    public PluginOwnerRepository(PluginsContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PluginOwner pluginOwner)
    {
        await _context.PluginOwners.AddAsync(pluginOwner);
    }

    public async Task<PluginOwner> GetByIdAsync(PluginOwnerId pluginOwnerId)
    {
        return await _context.PluginOwners.Include(p=>p.Plugins).FirstAsync(e=>e.Id == pluginOwnerId);
    }
}