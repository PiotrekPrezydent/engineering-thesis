using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using static Dara.Server.Modules.Plugins.IntegrationTests.ManifestSamples;

namespace Dara.Server.Modules.Plugins.IntegrationTests;

public class GetOwnerPluginsTests
{
    [Fact]
    public async Task OwnerSeesOnlyOwnPluginsWithTheirSettings()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files()]));
        await host.ExecuteAsync(new RegisterPluginsCommand(otherId, [Files(), Camera()]));

        var files = Assert.Single(await GetPluginsAsync(host, ownerId));
        var readFile = files.Functions.Single(f => f.Description.Name == "ReadFile");
        await host.ExecuteAsync(new DisablePluginFunctionCommand(ownerId, files.PluginId, readFile.FunctionId));

        var plugin = Assert.Single(await GetPluginsAsync(host, ownerId));
        Assert.Equal("Files", plugin.Name);
        Assert.Equal("File management", plugin.Description);
        Assert.Equal("1.0.0", plugin.Version);
        Assert.Equal(Hash('a'), plugin.Sha256);
        Assert.True(plugin.IsEnabled);
        Assert.Equal(["ReadFile", "SaveFile"], plugin.Functions.Select(f => f.Description.Name));
        Assert.Equal([false, true], plugin.Functions.Select(f => f.IsEnabled));
        Assert.Equal(Json(SaveFile), Json(plugin.Functions.Single(f => f.Description.Name == "SaveFile").Description));

        var other = await GetPluginsAsync(host, otherId);
        Assert.Equal(["Camera", "Files"], other.Select(p => p.Name));
        Assert.DoesNotContain(other, p => p.PluginId == plugin.PluginId);
        Assert.All(other.SelectMany(p => p.Functions), f => Assert.True(f.IsEnabled));
    }

    [Fact]
    public async Task UnknownOwnerHasNoPlugins()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);

        Assert.Empty(await GetPluginsAsync(host, Guid.NewGuid()));
    }

    private static Task<List<OwnerPluginDto>> GetPluginsAsync(PluginsModuleHost host, Guid ownerId)
        => host.QueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(ownerId));
}
