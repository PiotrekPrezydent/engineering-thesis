using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Orchestration.Infrastructure.Participants;

public class FunctionEntityTypeConfiguration : IEntityTypeConfiguration<Function>
{
    public void Configure(EntityTypeBuilder<Function> builder)
    {
        builder.ToTable(Function.DbTableName);
        
        builder.HasKey(x => x.Id);
        
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value, 
                value => new FunctionId(value));
        
        builder.OwnsMany(x => x.Parameters, parametersBuilder =>
        {
            parametersBuilder.ToJson();
        });
    }
}