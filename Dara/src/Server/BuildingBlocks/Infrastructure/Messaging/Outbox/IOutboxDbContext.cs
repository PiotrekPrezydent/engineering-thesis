using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public interface IOutboxDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; set; }
}