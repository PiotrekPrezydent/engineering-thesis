using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Infrastructure.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupsContext : ModuleContextBase
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMessage>  GroupMessages { get; set; }
    
    public GroupsContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroupsContext).Assembly);

        
        modelBuilder.Entity<Group>().HasData(
            SeedGroups.SeedAllGroups()
        );
        
        modelBuilder.Entity<GroupMember>()
            .HasData(
                SeedGroups.SeedAllGroupMembers()
        );
        
        base.OnModelCreating(modelBuilder);
    }
}