using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners.Plugins;

public class PluginFunctionEntityTypeConfiguration : IEntityTypeConfiguration<PluginFunction>
{
    public void Configure(EntityTypeBuilder<PluginFunction> builder)
    {
        builder.ToTable(PluginFunction.DbTableName);
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value, 
                value => new PluginFunctionId(value));
        
        builder.OwnsMany(x => x.Parameters, parametersBuilder =>
        {
            parametersBuilder.ToJson();
        });
    }
}