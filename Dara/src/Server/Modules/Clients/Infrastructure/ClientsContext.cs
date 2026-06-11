using Microsoft.EntityFrameworkCore;
using DbContext = Dara.BuildingBlocks.Infrastructure.DbContext;

namespace Dara.Server.Modules.Clients.Infrastructure;

public class ClientsContext : DbContext
{
    public ClientsContext(DbContextOptions options) : base(options)
    {
    }
}