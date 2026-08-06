using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupContext : ModuleContext
{
    protected GroupContext(DbContextOptions options) : base(options)
    {
    }
}