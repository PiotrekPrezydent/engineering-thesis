using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityContext : ModuleContext
{
    public DbSet<User> Users { get; set; }
    
    public IdentityContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}