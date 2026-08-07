using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupContext : ModuleContext
{
    public GroupContext(DbContextOptions options) : base(options)
    {
    }
}