using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Infrastructure;

public class PluginsContext : ModuleContextBase
{
    public DbSet<PluginOwner> PluginOwners { get; set; }
    
    public DbSet<Plugin> Plugins { get; set; }
    
    public DbSet<PluginConfiguration> PluginConfigurations { get; set; }
    
    public PluginsContext(DbContextOptions options) : base(options)
    {
    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PluginsContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}