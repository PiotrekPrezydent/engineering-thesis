using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Dara.Server.Modules.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityContext : ModuleContextBase
{
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    public IdentityContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
