using System.Collections.Concurrent;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Server.Modules.Identity.Application.ChangeClientContext;
using Dara.Server.Modules.Identity.Application.RenameClient;
using Dara.Server.Modules.Identity.Application.RevokeClient;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.IntegrationTests;

// the changes of a client leave identity through the outbox as UserClient* events; groups, orchestration and plugins
// build their projections of the client from them
public class ClientEventsTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task EveryChangeOfAClientIsPublishedWithItsAccount()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var recorder = new Recorder();
        host.EventBus.Subscribe<UserClientRegisteredIntegrationEvent>(recorder);
        host.EventBus.Subscribe<UserClientRenamedIntegrationEvent>(recorder);
        host.EventBus.Subscribe<UserClientContextChangedIntegrationEvent>(recorder);
        host.EventBus.Subscribe<UserClientRevokedIntegrationEvent>(recorder);
        var userId = await host.RegisterAsync("alice");

        var client = await host.RegisterClientAsync(userId, "phone");
        await host.ExecuteAsync(new RenameClientCommand(userId, client.ClientId, "Alice-Phone"));
        await host.ExecuteAsync(new ChangeClientContextCommand(userId, client.ClientId, new Dictionary<string, string> { ["City"] = "Poznan" }));
        await host.ExecuteAsync(new RevokeClientCommand(userId, client.ClientId));

        var registered = await recorder.WaitForAsync<UserClientRegisteredIntegrationEvent>();
        Assert.Equal((client.ClientId, userId, "Phone 1", "Phone"), (registered.ClientId, registered.UserId, registered.Name, registered.ClientType));
        var renamed = await recorder.WaitForAsync<UserClientRenamedIntegrationEvent>();
        Assert.Equal((client.ClientId, userId, "Alice-Phone"), (renamed.ClientId, renamed.UserId, renamed.NewName));
        var context = await recorder.WaitForAsync<UserClientContextChangedIntegrationEvent>();
        Assert.Equal(new Dictionary<string, string> { ["City"] = "Poznan" }, context.Context);
        var revoked = await recorder.WaitForAsync<UserClientRevokedIntegrationEvent>();
        Assert.Equal((client.ClientId, userId), (revoked.ClientId, revoked.UserId));
    }

    // a rename to the same name and the same context publish nothing
    [Fact]
    public async Task AChangeThatChangesNothingIsNotPublished()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var recorder = new Recorder();
        host.EventBus.Subscribe<UserClientRegisteredIntegrationEvent>(recorder);
        host.EventBus.Subscribe<UserClientRenamedIntegrationEvent>(recorder);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId, "pc", "Desk");

        await host.ExecuteAsync(new RenameClientCommand(userId, client.ClientId, " Desk "));
        await recorder.WaitForAsync<UserClientRegisteredIntegrationEvent>();
        await Task.Delay(300, TestContext.Current.CancellationToken);

        Assert.DoesNotContain(recorder.Events, e => e is UserClientRenamedIntegrationEvent);
    }

    private class Recorder :
        IIntegrationEventHandler<UserClientRegisteredIntegrationEvent>,
        IIntegrationEventHandler<UserClientRenamedIntegrationEvent>,
        IIntegrationEventHandler<UserClientContextChangedIntegrationEvent>,
        IIntegrationEventHandler<UserClientRevokedIntegrationEvent>
    {
        public ConcurrentQueue<IIntegrationEvent> Events { get; } = new();

        public Task HandleAsync(UserClientRegisteredIntegrationEvent integrationEvent) => Record(integrationEvent);
        public Task HandleAsync(UserClientRenamedIntegrationEvent integrationEvent) => Record(integrationEvent);
        public Task HandleAsync(UserClientContextChangedIntegrationEvent integrationEvent) => Record(integrationEvent);
        public Task HandleAsync(UserClientRevokedIntegrationEvent integrationEvent) => Record(integrationEvent);

        public async Task<TEvent> WaitForAsync<TEvent>()
        {
            using var timeout = new CancellationTokenSource(Timeout);
            while (true)
            {
                var found = Events.OfType<TEvent>().FirstOrDefault();
                if (found != null)
                    return found;

                try
                {
                    await Task.Delay(25, timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException($"not published within {Timeout.TotalSeconds} s: {typeof(TEvent).Name}");
                }
            }
        }

        private Task Record(IIntegrationEvent integrationEvent)
        {
            Events.Enqueue(integrationEvent);
            return Task.CompletedTask;
        }
    }
}
