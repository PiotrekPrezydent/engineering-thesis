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

    public object SeedGroup()
    {
        return new
        {
            Id = new GroupId(SharedSeedGuids.Group1),
            _ownerId = new GroupMemberId(SharedSeedGuids.User1),
            _name = "G1",
            _joinCode = "JG1"
        };
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