using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Infrastructure;

public class OrchestrationContext : ModuleContextBase
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<ParticipantGroup> ParticipantGroups { get; set; }
    
    public OrchestrationContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrchestrationContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}