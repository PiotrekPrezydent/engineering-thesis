using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Infrastructure;

public class AuthorizationContext : ModuleContextBase
{
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }

    public AuthorizationContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorizationContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
