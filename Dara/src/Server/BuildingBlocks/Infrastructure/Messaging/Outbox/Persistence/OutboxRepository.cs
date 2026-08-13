using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public class OutboxRepository<TDbcontext> : IOutboxRepository where TDbcontext : DbContext
{
    private readonly TDbcontext _dbContext;

    public OutboxRepository(TDbcontext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, CancellationToken ct)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .OrderBy(m => m.OccurredOn)
            .Select(m=>m.Id)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken ct)
    {
        await _dbContext.Set<OutboxMessage>().AddAsync(message, ct);
    }
}