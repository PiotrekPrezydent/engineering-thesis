using Dara.Server.Apps.API.Common.Presence;
using Dara.Server.Modules.Identity.Integration;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Notifications;

namespace Dara.Server.SystemTests.ClientServer;

// presence is the state of the hub connections of a client: online with the first one, offline after the last one
// closes cleanly or the server stops hearing from it; one event per change, with a growing sequence number
public class PresenceTests
{
    // short times, so a broken connection ends within the test
    private static readonly HubTiming ShortTiming = new(TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(2));

    [Fact]
    public async Task ASecondConnectionAndARenewalDoNotChangeThePresence()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var connected = RecordingIntegrationEventHandler<ClientConnectedIntegrationEvent>.SubscribeTo(host);
        var disconnected = RecordingIntegrationEventHandler<ClientDisconnectedIntegrationEvent>.SubscribeTo(host);
        var presence = host.Service<ClientPresenceTracker>();

        var phone = await host.ConnectAsync("present-phone", cancellationToken);
        Assert.True(presence.IsOnline(phone.ClientId));

        await phone.RenewAsync(cancellationToken);
        await host.RenameClientAsync(phone, "Renewed");
        await phone.Events.WaitForAsync<ClientRenamedNotification>(n => n.Name == "Renewed", "renamed on the renewed connection");

        Assert.True(presence.IsOnline(phone.ClientId));
        Assert.Equal(1, presence.ConnectionCount(phone.ClientId));
        var online = Assert.Single(connected.Events, e => e.ClientId == phone.ClientId);
        Assert.Equal(phone.UserId, online.UserId);
        Assert.DoesNotContain(disconnected.Events, e => e.ClientId == phone.ClientId);
    }

    [Fact]
    public async Task ACleanDisconnectionIsOfflineAtOnce()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var connected = RecordingIntegrationEventHandler<ClientConnectedIntegrationEvent>.SubscribeTo(host);
        var disconnected = RecordingIntegrationEventHandler<ClientDisconnectedIntegrationEvent>.SubscribeTo(host);
        var presence = host.Service<ClientPresenceTracker>();
        var laptop = await host.ConnectAsync("leaving-laptop", cancellationToken);

        await laptop.Connection.StopAsync(cancellationToken);

        await ModulesHost.WaitUntilAsync(() => Task.FromResult(!presence.IsOnline(laptop.ClientId)), "laptop offline", TimeSpan.FromSeconds(2));
        await ModulesHost.WaitUntilAsync(() => Task.FromResult(disconnected.Events.Any(e => e.ClientId == laptop.ClientId)), "disconnection published",
            TimeSpan.FromSeconds(2));
        var offline = Assert.Single(disconnected.Events, e => e.ClientId == laptop.ClientId);
        var online = Assert.Single(connected.Events, e => e.ClientId == laptop.ClientId);
        Assert.True(offline.Sequence > online.Sequence);
    }

    [Fact]
    public async Task ASilentClientIsOfflineAfterTheClientTimeout()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, timing: ShortTiming);
        var disconnected = RecordingIntegrationEventHandler<ClientDisconnectedIntegrationEvent>.SubscribeTo(host);
        var presence = host.Service<ClientPresenceTracker>();
        var silent = await host.ConnectAsync("silent-watch", cancellationToken, silent: true);
        var talking = await host.ConnectAsync("talking-phone", cancellationToken);
        Assert.True(presence.IsOnline(silent.ClientId));

        // the timeout plus a margin for a busy machine
        await ModulesHost.WaitUntilAsync(() => Task.FromResult(!presence.IsOnline(silent.ClientId)), "silent client offline",
            ShortTiming.ClientTimeout + TimeSpan.FromSeconds(5));

        Assert.Contains(disconnected.Events, e => e.ClientId == silent.ClientId);
        Assert.True(presence.IsOnline(talking.ClientId));
    }

    [Fact]
    public async Task ANotificationForOneClientReachesOnlyThatClientOfTheAccount()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var phone = await host.ConnectAsync("two-clients", cancellationToken);
        var laptop = await host.ConnectAnotherClientAsync(phone, cancellationToken);
        Assert.Equal(phone.UserId, laptop.UserId);

        await host.RenameClientAsync(phone, "Phone-Only");
        await host.RenameClientAsync(laptop, "Laptop-Only");

        await phone.Events.WaitForAsync<ClientRenamedNotification>(n => n.Name == "Phone-Only", "phone renamed");
        await laptop.Events.WaitForAsync<ClientRenamedNotification>(n => n.Name == "Laptop-Only", "laptop renamed");
        // signalr keeps the order of one connection: the laptop got its own rename after the rename of the phone
        Assert.DoesNotContain(laptop.Events.Received<ClientRenamedNotification>(), n => n.Name == "Phone-Only");
        Assert.DoesNotContain(phone.Events.Received<ClientRenamedNotification>(), n => n.Name == "Laptop-Only");
    }
}
