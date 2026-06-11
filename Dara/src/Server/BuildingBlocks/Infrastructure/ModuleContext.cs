using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure;

public class ModuleContext : DbContext
{
    protected ModuleContext(DbContextOptions options) : base(options)
    {
    }
}