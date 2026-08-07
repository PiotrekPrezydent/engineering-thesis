using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public class OutboxRepository<TDbcontext> : IOutboxRepository where TDbcontext : DbContext 
{
    private readonly TDbcontext _dbContext;

    public OutboxRepository(TDbcontext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetPendingMessagesAsync(CancellationToken ct)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .OrderBy(m => m.OccurredOn)
            .ToListAsync(ct);
    }

    public async Task MarkAsCompletedAsync(Guid messageId, CancellationToken ct)
    {
        var message = await _dbContext.Set<OutboxMessage>().FindAsync(new object[] { messageId }, ct);
        if (message != null)
        {
            message.ProcessedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task AddAndSaveAsync(OutboxMessage message, CancellationToken ct)
    {
        await _dbContext.Set<OutboxMessage>().AddAsync(message, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken ct)
    {
        await  _dbContext.Set<OutboxMessage>().AddAsync(message, ct);
    }
}