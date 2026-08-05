using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupModuleContext : ModuleContext
{
    protected GroupModuleContext(DbContextOptions options) : base(options)
    {
    }
}