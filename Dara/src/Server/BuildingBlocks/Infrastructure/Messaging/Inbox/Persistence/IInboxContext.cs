using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public interface IInboxContext
{
    public DbSet<InboxMessage> InboxMessages { get; set; }
}