using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging;

public class ModuleContext : DbContext
{
    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public ModuleContext(DbContextOptions options) : base(options)
    {
    
    }
}
