using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.SystemTests.ClientServer;

// a retried outbox message publishes its event again; the hub forwards every event to the client only once
public class HubNotificationsDeduplicationTests
{
    [Fact]
    public async Task RetriedOutboxMessageReachesTheClientOnce()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var failingOnce = new FailingOnceHandler();
        // the last subscriber of the bus, after the hub: the hub gets the event, then the outbox retries the message
        await using var host = await AppHubTestHost.StartAsync(cancellationToken,
            mapExtra: app => app.Services.GetRequiredService<IEventBus>().Subscribe(failingOnce));
        var phone = await host.ConnectAsync("dedup-phone", cancellationToken);

        await host.RenameClientAsync(phone, "Phone-1");
        await ModulesHost.WaitUntilAsync(() => Task.FromResult(failingOnce.Calls(n => n.NewName == "Phone-1") == 2), "outbox retried the rename");

        // signalr keeps the order of one connection and the hub forwards events in the order of the bus,
        // so after the second rename a repeat of the first one cannot come any more
        await host.RenameClientAsync(phone, "Phone-2");
        await phone.Events.WaitForAsync<ClientRenamedNotification>(n => n.Name == "Phone-2", "second rename");

        Assert.Single(phone.Events.Received<ClientRenamedNotification>(), n => n.Name == "Phone-1");
    }

    [Fact]
    public void DeduplicatorAcceptsAnEventOnceAndForgetsTheOldestWhenFull()
    {
        var deduplicator = new HubNotificationsDeduplicator(capacity: 2);
        var first = Renamed(Guid.NewGuid());

        Assert.True(deduplicator.TryAccept(first));
        // the outbox deserializes the message again, a new object with the same id
        Assert.False(deduplicator.TryAccept(new UserClientRenamedIntegrationEvent(first.EventId, first.OccurredOn, first.ClientId, first.UserId, first.NewName)));

        Assert.True(deduplicator.TryAccept(Renamed(Guid.NewGuid())));
        Assert.True(deduplicator.TryAccept(Renamed(Guid.NewGuid())));
        Assert.True(deduplicator.TryAccept(first));
    }

    [Fact]
    public void DeduplicatorKeysTheIdWithTheEventType()
    {
        var deduplicator = new HubNotificationsDeduplicator();
        var eventId = Guid.NewGuid();

        Assert.True(deduplicator.TryAccept(Renamed(eventId)));
        Assert.True(deduplicator.TryAccept(new UserClientRevokedIntegrationEvent(eventId, DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid())));
    }

    private static UserClientRenamedIntegrationEvent Renamed(Guid eventId)
    {
        return new UserClientRenamedIntegrationEvent(eventId, DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid(), "name");
    }

    // fails the first delivery of every event, the bus calls it from the outbox worker of identity
    private class FailingOnceHandler : IIntegrationEventHandler<UserClientRenamedIntegrationEvent>
    {
        private readonly Lock _lock = new();
        private readonly List<UserClientRenamedIntegrationEvent> _calls = new();

        public int Calls(Func<UserClientRenamedIntegrationEvent, bool> match)
        {
            lock (_lock)
                return _calls.Count(match);
        }

        public Task HandleAsync(UserClientRenamedIntegrationEvent integrationEvent)
        {
            lock (_lock)
            {
                var first = _calls.All(c => c.EventId != integrationEvent.EventId);
                _calls.Add(integrationEvent);
                if (first)
                    throw new InvalidOperationException("a subscriber of the bus failed once");
            }

            return Task.CompletedTask;
        }
    }
}
