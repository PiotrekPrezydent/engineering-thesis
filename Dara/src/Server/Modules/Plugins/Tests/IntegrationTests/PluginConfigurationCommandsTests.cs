using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;
using static Dara.Server.Modules.Plugins.IntegrationTests.ManifestSamples;

namespace Dara.Server.Modules.Plugins.IntegrationTests;

public class PluginConfigurationCommandsTests
{
    [Fact]
    public async Task DisabledFunctionLeavesThePublishedStateAndEnabledComesBack()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();
        var files = await RegisterFilesAsync(host, ownerId);
        var saveFile = files.Functions.Single(f => f.Description.Name == "SaveFile");

        await host.ExecuteAsync(new DisablePluginFunctionCommand(ownerId, files.PluginId, saveFile.FunctionId));

        var disabled = await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 2, "state without the function");
        Assert.Equal(["ReadFile"], disabled.Functions.Select(f => f.Description.Name));
        Assert.False((await GetFilesAsync(host, ownerId)).Functions.Single(f => f.FunctionId == saveFile.FunctionId).IsEnabled);

        await host.ExecuteAsync(new EnablePluginFunctionCommand(ownerId, files.PluginId, saveFile.FunctionId));

        var enabled = await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 3, "state with the function again");
        Assert.Contains(enabled.Functions, f => f.FunctionId == saveFile.FunctionId);
        Assert.True((await GetFilesAsync(host, ownerId)).Functions.Single(f => f.FunctionId == saveFile.FunctionId).IsEnabled);
    }

    [Fact]
    public async Task DisabledPluginPublishesNoFunctionsAndKeepsTheirSettings()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();
        var files = await RegisterFilesAsync(host, ownerId);
        var readFile = files.Functions.Single(f => f.Description.Name == "ReadFile");

        await host.ExecuteAsync(new DisablePluginFunctionCommand(ownerId, files.PluginId, readFile.FunctionId));
        await host.ExecuteAsync(new DisablePluginCommand(ownerId, files.PluginId));

        var disabled = await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 3, "state of the disabled plugin");
        Assert.Empty(disabled.Functions);
        Assert.False((await GetFilesAsync(host, ownerId)).IsEnabled);

        await host.ExecuteAsync(new EnablePluginCommand(ownerId, files.PluginId));

        // the function disabled before stays disabled
        var enabled = await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 4, "state of the enabled plugin");
        Assert.Equal(["SaveFile"], enabled.Functions.Select(f => f.Description.Name));
    }

    [Fact]
    public async Task EveryChangePublishesAHigherVersion()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();
        var files = await RegisterFilesAsync(host, ownerId);

        await host.ExecuteAsync(new DisablePluginCommand(ownerId, files.PluginId));
        await host.ExecuteAsync(new EnablePluginCommand(ownerId, files.PluginId));
        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, []));

        await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 4, "state after the removal");

        // a late notification publishes the state the plugin has by then (PublishAvailableFunctionsHandler), so a
        // version can be skipped or repeated, but no subscriber gets a lower version after a higher one
        var versions = host.Published.Where(e => e.PluginId == files.PluginId).Select(e => e.Version).ToList();
        Assert.Equal(versions.Order(), versions);
        Assert.Equal(4, versions[^1]);
    }

    [Fact]
    public async Task StrangerCannotConfigureThePlugin()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();
        var strangerId = Guid.NewGuid();
        var files = await RegisterFilesAsync(host, ownerId);
        var saveFile = files.Functions.First();

        await AssertBrokenAsync<OnlyPluginOwnerCanConfigurePluginRule>(() => host.ExecuteAsync(new DisablePluginCommand(strangerId, files.PluginId)));
        await AssertBrokenAsync<OnlyPluginOwnerCanConfigurePluginRule>(() => host.ExecuteAsync(new EnablePluginCommand(strangerId, files.PluginId)));
        await AssertBrokenAsync<OnlyPluginOwnerCanConfigurePluginRule>(() => host.ExecuteAsync(new DisablePluginFunctionCommand(strangerId, files.PluginId, saveFile.FunctionId)));
        await AssertBrokenAsync<OnlyPluginOwnerCanConfigurePluginRule>(() => host.ExecuteAsync(new EnablePluginFunctionCommand(strangerId, files.PluginId, saveFile.FunctionId)));

        var unchanged = await GetFilesAsync(host, ownerId);
        Assert.True(unchanged.IsEnabled);
        Assert.All(unchanged.Functions, f => Assert.True(f.IsEnabled));
    }

    [Fact]
    public async Task FunctionOfAnotherPluginIsRejected()
    {
        await using var host = await PluginsModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = Guid.NewGuid();
        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files(), Camera()]));
        var plugins = await host.QueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(ownerId));
        var files = plugins.Single(p => p.Name == "Files");
        var takePhoto = plugins.Single(p => p.Name == "Camera").Functions.Single();

        await AssertBrokenAsync<FunctionMustBelongToPluginRule>(() =>
            host.ExecuteAsync(new DisablePluginFunctionCommand(ownerId, files.PluginId, takePhoto.FunctionId)));
    }

    private static async Task<OwnerPluginDto> RegisterFilesAsync(PluginsModuleHost host, Guid ownerId)
    {
        await host.ExecuteAsync(new RegisterPluginsCommand(ownerId, [Files()]));
        var files = await GetFilesAsync(host, ownerId);
        await host.WaitForPublishedAsync(e => e.PluginId == files.PluginId && e.Version == 1, "first state of files");

        return files;
    }

    private static async Task<OwnerPluginDto> GetFilesAsync(PluginsModuleHost host, Guid ownerId)
    {
        var plugins = await host.QueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(ownerId));
        return plugins.Single(p => p.Name == "Files");
    }

    private static async Task AssertBrokenAsync<TRule>(Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
    }
}
