using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners.Plugins;

public class PluginEntityTypeConfiguration : IEntityTypeConfiguration<Plugin>
{
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        builder.ToTable(Plugin.DbTableName);
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value, 
                value => new PluginId(value));
        
        builder.HasMany(x => x.Functions)
            .WithOne(x => x.Plugin)
            .HasForeignKey(x => x.PluginId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Functions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}