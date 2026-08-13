using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Profiles.Domain;
using Dara.Server.Modules.Profiles.Infrastructure.Profiles;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Profiles.Infrastructure;

public class ProfilesContext : ModuleContextBase
{
    public DbSet<Profile> Profiles { get; set; }
    
    public ProfilesContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfilesContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}