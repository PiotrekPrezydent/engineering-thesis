using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
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
        throw new NotImplementedException();
    }

    public async Task<PluginConfiguration> GetByIdAsync(PluginConfigurationId id)
    {
        return await _context.PluginConfigurations.FirstAsync(e=>e.Id == id);
    }

    public async Task RemoveAsync(PluginConfiguration pluginConfiguration)
    {
        _context.PluginConfigurations.Remove(pluginConfiguration);
    }
}