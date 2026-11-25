using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.Modules.Identity.Integration;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Microsoft.EntityFrameworkCore;
using static Dara.Server.Modules.Plugins.IntegrationTests.ManifestSamples;

namespace Dara.Server.Modules.Plugins.IntegrationTests;

public class RegisterPluginsTests
{
    [Fact]
    public async Task NewPluginsAreStoredEnabledAndPublished()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files(), Camera()]));

        var plugins = await GetPluginsAsync(host, ownerId);
        Assert.Equal(["Camera", "Files"], plugins.Select(p => p.Name));
        Assert.All(plugins, p => Assert.True(p.IsEnabled));
        Assert.All(plugins.SelectMany(p => p.Functions), f => Assert.True(f.IsEnabled));

        foreach (var plugin in plugins)
        {
            var published = await host.WaitForPublishedAsync(e => e.PluginId == plugin.PluginId, $"state of {plugin.Name}");

            Assert.Equal(ownerId, published.PluginOwnerId);
            Assert.Equal(1, published.Version);
            Assert.Equal(plugin.Functions.Select(f => f.FunctionId).Order(), published.Functions.Select(f => f.FunctionId).Order());
        }
    }

    [Fact]
    public async Task PublishedStateKeepsTheFullDescription()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files(), Camera()]));

        var files = await host.WaitForPublishedAsync(e => e.Functions.Count == 2, "state of files");
        var camera = await host.WaitForPublishedAsync(e => e.Functions.Count == 1, "state of camera");

        Assert.Equal(Json(SaveFile), Json(files.Functions.Single(f => f.Description.Name == "SaveFile").Description));
        Assert.Equal(Json(ReadFile), Json(files.Functions.Single(f => f.Description.Name == "ReadFile").Description));
        Assert.Equal(Json(TakePhoto), Json(camera.Functions.Single().Description));
    }

    [Fact]
    public async Task SameSetTwiceChangesNothing()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files(), Camera()]));
        var before = await GetPluginsAsync(host, ownerId);
        var outboxBefore = await CountOutboxAsync(host);

        // the hash is compared without case
        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Camera(), Files() with { Sha256 = Hash('A') }]));

        var after = await GetPluginsAsync(host, ownerId);
        Assert.Equal(before.Select(p => p.PluginId), after.Select(p => p.PluginId));
        Assert.Equal(before.SelectMany(p => p.Functions).Select(f => f.FunctionId), after.SelectMany(p => p.Functions).Select(f => f.FunctionId));
        Assert.Equal(outboxBefore, await CountOutboxAsync(host));
    }

    [Fact]
    public async Task NewHashReplacesPluginOfTheSameName()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files('a')]));
        var old = Assert.Single(await GetPluginsAsync(host, ownerId));

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files('b')]));
        var replacement = Assert.Single(await GetPluginsAsync(host, ownerId));

        Assert.NotEqual(old.PluginId, replacement.PluginId);
        Assert.Equal(Hash('b'), replacement.Sha256);
        Assert.False(await ConfigurationExistsAsync(host, old.PluginId));

        var removed = await host.WaitForPublishedAsync(e => e.PluginId == old.PluginId && e.Version == 2, "empty state of the old plugin");
        Assert.Empty(removed.Functions);

        var added = await host.WaitForPublishedAsync(e => e.PluginId == replacement.PluginId, "state of the new plugin");
        Assert.Equal(1, added.Version);
        Assert.Equal(2, added.Functions.Count);
    }

    [Fact]
    public async Task MissingPluginIsRemovedWithItsConfiguration()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files(), Camera()]));
        var files = (await GetPluginsAsync(host, ownerId)).Single(p => p.Name == "Files");

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Camera()]));

        Assert.Equal(["Camera"], (await GetPluginsAsync(host, ownerId)).Select(p => p.Name));
        Assert.False(await ConfigurationExistsAsync(host, files.PluginId));
        Assert.False(await host.ReadAsync(model => model.Query<Plugin>().AnyAsync(p => p.Id.Match(files.PluginId))));

        var removed = await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 2, "empty state of the removed plugin");
        Assert.Empty(removed.Functions);
    }

    [Fact]
    public async Task RegistrationBeforeNewUserEventCreatesTheOwner()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files()]));
        Assert.Equal(1, await CountOwnersAsync(host, ownerId));

        var userCreated = new NewUserCreatedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, ownerId);
        await host.EventBus.PublishAsync(userCreated);

        // a failing handler leaves the message unprocessed
        await PluginsModuleHost.WaitUntilAsync(() => host.ReadAsync(model => model.Query<InboxMessage>()
            .AnyAsync(m => m.Id == userCreated.EventId && m.ProcessedDate != null)), "new user event processed");

        Assert.Equal(1, await CountOwnersAsync(host, ownerId));
        Assert.Single(await GetPluginsAsync(host, ownerId));
    }

    [Fact]
    public async Task InvalidManifestStoresNothing()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files(), Camera() with { Sha256 = "not-a-hash" }])));

        Assert.IsType<PluginHashMustBeSha256Rule>(exception.BrokenRule);
        Assert.Empty(await GetPluginsAsync(host, ownerId));
        Assert.Equal(0, await CountOwnersAsync(host, ownerId));
    }

    private static Task<List<OwnerPluginDto>> GetPluginsAsync(PluginsModuleHost host, Guid ownerId)
        => host.QueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(ownerId));

    private static Task<int> CountOutboxAsync(PluginsModuleHost host)
        => host.ReadAsync(model => model.Query<OutboxMessage>().CountAsync());

    private static Task<int> CountOwnersAsync(PluginsModuleHost host, Guid ownerId)
        => host.ReadAsync(model => model.Query<PluginOwner>().CountAsync(o => o.Id.Match(ownerId)));

    private static Task<bool> ConfigurationExistsAsync(PluginsModuleHost host, Guid pluginId)
        => host.ReadAsync(model => model.Query<PluginConfiguration>().AnyAsync(c => c.PluginId.Match(pluginId)));
}
