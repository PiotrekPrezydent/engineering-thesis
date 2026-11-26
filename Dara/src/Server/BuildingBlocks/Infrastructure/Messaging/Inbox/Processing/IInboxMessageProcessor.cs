namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public interface IInboxMessageProcessor
{
    Task ProcessSingleMessageAsync(Guid messageId, CancellationToken stoppingToken);

    // call in a new scope after ProcessSingleMessageAsync threw, so nothing tracked by the failed attempt is saved
    Task RecordFailureAsync(Guid messageId, Exception error, CancellationToken ct);
}
