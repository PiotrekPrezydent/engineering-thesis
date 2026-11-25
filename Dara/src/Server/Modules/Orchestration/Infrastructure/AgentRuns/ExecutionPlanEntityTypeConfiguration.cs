using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns;

public class ExecutionPlanEntityTypeConfiguration : IEntityTypeConfiguration<ExecutionPlan>
{
    public void Configure(EntityTypeBuilder<ExecutionPlan> builder)
    {
        builder.ToTable(ExecutionPlan.DbTableName);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ExecutionPlanId(value));

        // calls are added in place while the plan is generated, the json comparer sees each one
        builder.Property(x => x.Calls)
            .HasField("_calls")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasJsonConversion();
    }
}
