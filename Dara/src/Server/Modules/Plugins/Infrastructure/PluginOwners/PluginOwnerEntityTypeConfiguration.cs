using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners;

public class PluginOwnerEntityTypeConfiguration : IEntityTypeConfiguration<PluginOwner>
{
    public void Configure(EntityTypeBuilder<PluginOwner> builder)
    {
        builder.ToTable(PluginOwner.DbTableName);
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value, 
                value => new PluginOwnerId(value));

        builder.HasMany(x => x.Plugins)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(x => x.Plugins)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}