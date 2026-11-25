using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginConfigurations;

public class PluginConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<PluginConfiguration>
{
    public void Configure(EntityTypeBuilder<PluginConfiguration> builder)
    {
        builder.ToTable(PluginConfiguration.DbTableName);
        
        builder.HasKey(p => p.Id);

        // one configuration per plugin
        builder.HasIndex(p => p.PluginId).IsUnique();

        builder.HasMany(p => p.FunctionConfigurations)
            .WithOne(f => f.PluginConfiguration)
            .HasForeignKey(f => f.PluginConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.FunctionConfigurations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}