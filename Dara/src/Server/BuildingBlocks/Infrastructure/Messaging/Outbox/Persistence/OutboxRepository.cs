using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public class OutboxRepository<TDbcontext> : IOutboxRepository where TDbcontext : DbContext
{
    private readonly TDbcontext _dbContext;

    public OutboxRepository(TDbcontext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, DateTime now, CancellationToken ct)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedDate == null && m.NextAttemptAt <= now)
            .OrderBy(m => m.OccurredOn)
            .Select(m=>m.Id)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public async Task<DateTime?> GetNextAttemptAtAsync(CancellationToken ct)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .Select(m => (DateTime?)m.NextAttemptAt)
            .MinAsync(ct);
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken ct)
    {
        await _dbContext.Set<OutboxMessage>().AddAsync(message, ct);
    }
}