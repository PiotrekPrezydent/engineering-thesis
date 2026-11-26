using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public class InboxRepository<TDbcontext> : IInboxRepository where TDbcontext : DbContext
{
    private readonly TDbcontext _context;

    public InboxRepository(TDbcontext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, DateTime now, CancellationToken ct)
    {
        return await _context.Set<InboxMessage>()
            .Where(m => m.ProcessedDate == null && m.NextAttemptAt <= now)
            .OrderBy(m => m.OccurredOn)
            .Select(m=>m.Id)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public async Task<DateTime?> GetNextAttemptAtAsync(CancellationToken ct)
    {
        return await _context.Set<InboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .Select(m => (DateTime?)m.NextAttemptAt)
            .MinAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid messageId, CancellationToken ct)
    {
        return await _context.Set<InboxMessage>().AnyAsync(m => m.Id == messageId, ct);
    }

    public async Task AddAsync(InboxMessage message, CancellationToken ct)
    {
        await _context.Set<InboxMessage>().AddAsync(message, ct);
    }
}