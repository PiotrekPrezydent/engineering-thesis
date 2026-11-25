using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns;

public class AgentRunEntityTypeConfiguration : IEntityTypeConfiguration<AgentRun>
{
    public void Configure(EntityTypeBuilder<AgentRun> builder)
    {
        builder.ToTable(AgentRun.DbTableName);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new AgentRunId(value));

        builder.Property(x => x.Status)
            .HasConversion(
                status => status.Value,
                value => new AgentRunStatus(value));

        // snapshots and aliases never change after the run starts
        builder.Property(x => x.GroupSnapshot).HasJsonConversion();
        builder.Property(x => x.ParticipantsSnapshots).HasJsonConversion();
        builder.Property(x => x.ToolAliases).HasJsonConversion();

        // the domain raises it on every change, a save with an older value fails
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasOne(x => x.Plan)
            .WithOne()
            .HasForeignKey<ExecutionPlan>(x => x.AgentRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Plan).IsRequired();
    }
}
