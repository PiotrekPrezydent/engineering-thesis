using Microsoft.EntityFrameworkCore;

namespace Dara.BuildingBlocks.Infrastructure;

public class ModuleContext : DbContext
{
    protected ModuleContext(DbContextOptions options) : base(options)
    {
    }
}