using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.UnitTests;

public class MessagingTests
{
    private static TestModule ModuleWithNotificationHandlers() => new(configure: services =>
    {
        // the order matters: the command is queued before the second handler fails
        services.AddScoped<IDomainEventNotificationHandler<TestNotification>, QueueCommandOnNotificationHandler>();
        services.AddScoped<IDomainEventNotificationHandler<TestNotification>, FlakyNotificationHandler>();
    });

    private static async Task<Guid> AddOutboxMessageAsync(TestModule module, string name)
    {
        var notification = new TestNotification(Guid.NewGuid(), new TestDomainEvent(name));
        var message = new OutboxMessage(notification.NotificationId, DateTime.UtcNow, nameof(TestNotification), JsonSerializer.Serialize(notification));

        await module.ReadAsync(async db =>
        {
            db.OutboxMessages.Add(message);
            return await db.SaveChangesAsync();
        });
        module.GetRequiredService<OutboxQueueSignal>().NotifyNewMessage();

        return message.Id;
    }

    private static Task<OutboxMessage> GetOutboxMessageAsync(TestModule module, Guid id) =>
        module.ReadAsync(db => db.OutboxMessages.AsNoTracking().SingleAsync(m => m.Id == id));

    [Fact]
    public async Task RetriedOutboxMessageQueuesItsCommandExactlyOnce()
    {
        await using var module = ModuleWithNotificationHandlers();
        module.Probe.FailFirst("notification:retried", 2);
        await module.StartAsync<OutboxBackgroundWorker>();
        await module.StartAsync<InternalCommandsBackgroundWorker>();

        var messageId = await AddOutboxMessageAsync(module, "retried");
        await module.WaitUntilAsync(async () => (await GetOutboxMessageAsync(module, messageId)).ProcessedDate != null, "outbox message processed");
        await module.WaitUntilProcessedAsync("retried");

        var message = await GetOutboxMessageAsync(module, messageId);
        Assert.Equal(2, message.Attempts);
        Assert.Contains("failure 2 of notification:retried", message.LastError);
        Assert.Equal(1, await module.ReadAsync(db => db.InternalCommands.CountAsync()));
        Assert.Equal(1, await module.CountEffectsAsync("retried"));
    }

    [Fact]
    public async Task LongInternalCommandDoesNotHoldBackTheOutbox()
    {
        await using var module = ModuleWithNotificationHandlers();
        module.Probe.Hold("long");
        await module.StartAsync<OutboxBackgroundWorker>();
        await module.StartAsync<InternalCommandsBackgroundWorker>();

        await AddOutboxMessageAsync(module, "long");
        await module.WaitUntilAsync(() => Task.FromResult(module.Probe.Started.Contains("long")), "long command started");

        var nextId = await AddOutboxMessageAsync(module, "next");
        await module.WaitUntilAsync(async () => (await GetOutboxMessageAsync(module, nextId)).ProcessedDate != null, "next outbox message processed");
        await module.WaitUntilProcessedAsync("next");

        Assert.Null((await module.GetCommandAsync("long")).ProcessedDate);

        module.Probe.Release("long");
        await module.WaitUntilProcessedAsync("long");
    }

    [Fact]
    public async Task InboxKeepsOneMessageWhenTheEventComesTwice()
    {
        await using var module = new TestModule();
        var writer = new InboxWriterIntegrationEventHandler<TestIntegrationEvent>(module);
        var integrationEvent = new TestIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, "twice");

        await writer.HandleAsync(integrationEvent);
        await writer.HandleAsync(integrationEvent);

        Assert.Equal(1, await module.ReadAsync(db => db.InboxMessages.CountAsync()));
    }

    [Fact]
    public async Task FailingSubscriberDoesNotCutOffTheNextOnes()
    {
        var bus = new InMemoryEventBus();
        var received = new Recorder();
        bus.Subscribe(new Failing());
        bus.Subscribe(received);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => bus.PublishAsync(new TestIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, "event")));

        Assert.Equal("subscriber failed", error.Message);
        Assert.Equal(["event"], received.Names);
    }

    private class Failing : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public Task HandleAsync(TestIntegrationEvent integrationEvent) => throw new InvalidOperationException("subscriber failed");
    }

    private class Recorder : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public List<string> Names { get; } = [];

        public Task HandleAsync(TestIntegrationEvent integrationEvent)
        {
            Names.Add(integrationEvent.Name);
            return Task.CompletedTask;
        }
    }
}
