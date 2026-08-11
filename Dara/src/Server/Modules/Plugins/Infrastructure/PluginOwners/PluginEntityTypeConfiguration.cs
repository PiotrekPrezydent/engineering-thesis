using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners;

public class PluginEntityTypeConfiguration : IEntityTypeConfiguration<Plugin>
{
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value, 
                value => new PluginId(value));

        builder.Property(p => p.OwnerId)
            .HasConversion(
                id => id.Value, 
                value => new PluginOwnerId(value));

        // builder.OwnsMany(p => p.Functions, funcBuilder =>
        // {
        //     funcBuilder.Property<int>("Id");
        //     funcBuilder.HasKey("Id");
        //     
        //     funcBuilder.WithOwner().HasForeignKey("PluginId");
        //
        //     funcBuilder.OwnsMany(f => f.Parameters, paramBuilder =>
        //     {
        //         paramBuilder.Property<int>("Id");
        //         paramBuilder.HasKey("Id");
        //         
        //         paramBuilder.WithOwner().HasForeignKey("PluginFunctionId");
        //     });
        // });
    }
}