using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityContext : ModuleContextBase
{
    public DbSet<User> Users { get; set; }
    public DbSet<DeviceSession> DeviceSessions { get; set; }
    public DbSet<AccountSession> AccountSessions { get; set; }

    public IdentityContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
