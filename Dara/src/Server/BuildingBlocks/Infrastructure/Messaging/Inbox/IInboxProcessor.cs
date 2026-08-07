namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public interface IInboxProcessor
{
    Task ProcessInboxAsync(CancellationToken stoppingToken);
}