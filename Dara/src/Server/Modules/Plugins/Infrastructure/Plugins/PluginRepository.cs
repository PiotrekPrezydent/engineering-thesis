using Dara.Server.Modules.Plugins.Domain.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Infrastructure.Plugins;

public class PluginRepository : IPluginRepository
{
    private readonly PluginsContext _context;

    public PluginRepository(PluginsContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Plugin plugin)
    {
        await _context.Plugins.AddAsync(plugin);
    }

    public async Task<Plugin> GetByIdAsync(PluginId id)
    {
        return await _context.Plugins.FirstAsync(e=>e.Id == id);
    }

    public async Task RemoveAsync(Plugin plugin)
    {
        _context.Plugins.Remove(plugin);
    }
}