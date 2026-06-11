using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.Modules.Clients.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Clients.Infrastructure;

public class ClientsContext : ModuleContext
{
    public DbSet<Client>  Clients { get; set; }
    public ClientsContext(DbContextOptions<ClientsContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientsContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}