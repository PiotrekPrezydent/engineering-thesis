using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public interface IOutboxContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
}