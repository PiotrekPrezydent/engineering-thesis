using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners;

public class PluginOwnerEntityTypeConfiguration : IEntityTypeConfiguration<PluginOwner>
{
    public void Configure(EntityTypeBuilder<PluginOwner> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value, 
                value => new PluginOwnerId(value));

        builder.HasMany(o => o.Plugins)
            .WithOne()
            .HasForeignKey(p => p.OwnerId);
        
        builder.Metadata.FindNavigation(nameof(PluginOwner.Plugins))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
    }
}