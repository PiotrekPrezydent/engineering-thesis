namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public interface IInboxProcessor
{
    Task ProcessInboxAsync(CancellationToken stoppingToken);
}