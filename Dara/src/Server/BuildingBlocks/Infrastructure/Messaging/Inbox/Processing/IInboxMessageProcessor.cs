namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public interface IInboxMessageProcessor
{
    Task ProcessSingleMessageAsync(Guid messageId, CancellationToken stoppingToken);
}