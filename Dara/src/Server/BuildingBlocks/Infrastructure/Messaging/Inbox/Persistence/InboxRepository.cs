using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public class InboxRepository<TDbcontext> : IInboxRepository where TDbcontext : DbContext
{
    private readonly TDbcontext _context;

    public InboxRepository(TDbcontext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, CancellationToken ct)
    {
        return await _context.Set<InboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .OrderBy(m => m.OccurredOn)
            .Select(m=>m.Id)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public async Task AddAsync(InboxMessage message, CancellationToken ct)
    {
        await _context.Set<InboxMessage>().AddAsync(message, ct);
    }
}