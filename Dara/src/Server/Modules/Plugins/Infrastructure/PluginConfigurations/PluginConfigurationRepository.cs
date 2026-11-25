using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginConfigurations;

public class PluginConfigurationRepository : IPluginConfigurationRepository
{
    private readonly PluginsContext _context;

    public PluginConfigurationRepository(PluginsContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PluginConfiguration pluginConfiguration)
    {
        await _context.PluginConfigurations.AddAsync(pluginConfiguration);
    }

    public async Task<PluginConfiguration> GetByIdAsync(PluginConfigurationId id)
    {
        return await _context.PluginConfigurations.Include(e => e.FunctionConfigurations).FirstAsync(e => e.Id == id);
    }

    public async Task<PluginConfiguration> GetByPluginIdAsync(PluginId pluginId)
    {
        return await _context.PluginConfigurations.Include(e => e.FunctionConfigurations).FirstAsync(e => e.PluginId == pluginId);
    }

    public async Task RemoveAsync(PluginConfiguration pluginConfiguration)
    {
        _context.PluginConfigurations.Remove(pluginConfiguration);
    }
}