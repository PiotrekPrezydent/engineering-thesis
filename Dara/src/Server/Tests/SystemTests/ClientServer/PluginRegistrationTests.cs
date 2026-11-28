using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.Plugins;
using System.Net;
using Dara.Clients.Core.Api;

namespace Dara.Server.SystemTests.ClientServer;

// plugins registered over http, the owner gets its registered set back over the hub; the panel configures them
public class PluginRegistrationTests
{
    private static PluginManifest Camera => new("Camera", "Camera of the device", "1.0.0", new string('c', 64),
        [new FunctionDescription("TakePhoto", "Takes a photo", [], new TypeDescription(TypeKind.String, ContentEncoding: "base64"))]);

    private static PluginManifest Files => new("Files", null, "1.0.0", new string('f', 64),
    [
        new FunctionDescription("SaveFile", "Saves a file",
        [
            new PropertyDescription("path", new TypeDescription(TypeKind.String), true),
            new PropertyDescription("content", new TypeDescription(TypeKind.String), true)
        ], new TypeDescription(TypeKind.Boolean)),
        new FunctionDescription("DeleteFile", "Deletes a file",
            [new PropertyDescription("path", new TypeDescription(TypeKind.String), true)], null)
    ]);

    private static PluginManifest Notes => new("Notes", null, "2.1.0", new string('a', 64),
        [new FunctionDescription("AddNote", null, [new PropertyDescription("text", new TypeDescription(TypeKind.String), true)], null)]);

    [Fact]
    public async Task TwoClientsRegisterTheirPluginsAndAReconnectionDoesNotDuplicateThem()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var phone = await host.ConnectAsync("alice-phone", cancellationToken);
        var laptop = await host.ConnectAsync("alice-laptop", cancellationToken);

        await phone.Api.RegisterPluginsAsync([Camera], cancellationToken);
        await laptop.Api.RegisterPluginsAsync([Files, Notes], cancellationToken);

        await phone.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 1, "camera registered");
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 2, "files and notes registered");

        var phonePlugins = await phone.Api.GetCurrentClientPluginsAsync(cancellationToken);
        var laptopPlugins = await laptop.Api.GetCurrentClientPluginsAsync(cancellationToken);
        Assert.Equal(["Camera"], phonePlugins.Select(p => p.Name));
        Assert.Equal(["Files", "Notes"], laptopPlugins.Select(p => p.Name));
        Assert.Equal(["DeleteFile", "SaveFile"], laptopPlugins[0].Functions.Select(f => f.Description.Name));
        Assert.All(laptopPlugins.SelectMany(p => p.Functions), f => Assert.True(f.IsEnabled));
        Assert.DoesNotContain(laptop.Events.Received<PluginsRegisteredNotification>(), n => n.Plugins.Any(p => p.Name == "Camera"));

        // the cli sends its whole set again after every connection
        await phone.ReconnectAsync(cancellationToken);
        await phone.Api.RegisterPluginsAsync([Camera], cancellationToken);

        var afterReconnection = await phone.Api.GetCurrentClientPluginsAsync(cancellationToken);
        var camera = Assert.Single(afterReconnection);
        Assert.Equal(phonePlugins[0].PluginId, camera.PluginId);
    }

    [Fact]
    public async Task NewSetReplacesTheRegisteredOne()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var laptop = await host.ConnectAsync("replacing-laptop", cancellationToken);

        await laptop.Api.RegisterPluginsAsync([Files, Notes], cancellationToken);
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 2, "files and notes registered");
        var notesBefore = (await laptop.Api.GetCurrentClientPluginsAsync(cancellationToken)).Single(p => p.Name == "Notes");

        var newNotes = Notes with { Version = "2.2.0", Sha256 = new string('b', 64) };
        await laptop.Api.RegisterPluginsAsync([newNotes], cancellationToken);

        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 1 && n.Plugins[0].Version == "2.2.0", "only the new notes");
        var notesAfter = Assert.Single(await laptop.Api.GetCurrentClientPluginsAsync(cancellationToken));
        Assert.NotEqual(notesBefore.PluginId, notesAfter.PluginId);
    }

    [Fact]
    public async Task OwnerDisablesAndEnablesAFunctionAndOthersCannot()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var laptop = await host.ConnectAsync("owner-laptop", cancellationToken);
        var stranger = await host.ConnectAsync("stranger-phone", cancellationToken);

        await laptop.Api.RegisterPluginsAsync([Files], cancellationToken);
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 1, "files registered");
        var files = Assert.Single(await laptop.Api.GetCurrentClientPluginsAsync(cancellationToken));
        var deleteFile = files.Functions.Single(f => f.Description.Name == "DeleteFile");

        // the panel of another account does not see the laptop
        var strangerPanel = await host.PanelOfAsync(stranger);
        var error = await PanelTestClient.ProblemAsync(await strangerPanel.DisableFunctionAsync(laptop.ClientId, files.PluginId, deleteFile.FunctionId));
        Assert.Equal((404, "client_not_found"), (error.Status, error.Code));

        var panel = await host.PanelOfAsync(laptop);
        await AppHubTestHost.EnsureAsync(await panel.DisableFunctionAsync(laptop.ClientId, files.PluginId, deleteFile.FunctionId), "delete file disabled");
        var twice = await PanelTestClient.ProblemAsync(await panel.DisableFunctionAsync(laptop.ClientId, files.PluginId, deleteFile.FunctionId));
        Assert.Equal((409, "conflict", "OnlyActivePluginFunctionCanBeDisabledRule"), (twice.Status, twice.Code, twice.Rule));
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(
            n => n.Plugins.Single().Functions.Any(f => f.FunctionId == deleteFile.FunctionId && !f.IsEnabled), "delete file disabled");
        await ModulesHost.WaitUntilAsync(async () => (await host.GetParticipantFunctionNamesAsync(laptop.ClientId)).SequenceEqual(["SaveFile"]),
            "only save file left in orchestration");

        await AppHubTestHost.EnsureAsync(await panel.EnableFunctionAsync(laptop.ClientId, files.PluginId, deleteFile.FunctionId), "delete file enabled");
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(
            n => n.Plugins.Single().Functions.All(f => f.IsEnabled), "delete file enabled again");
        await ModulesHost.WaitUntilAsync(async () => (await host.GetParticipantFunctionNamesAsync(laptop.ClientId)).SequenceEqual(["DeleteFile", "SaveFile"]),
            "delete file back in orchestration");
    }

    [Fact]
    public async Task BrokenRuleOfARegistrationIsAnErrorForTheCaller()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var phone = await host.ConnectAsync("bad-plugin-phone", cancellationToken);

        var error = await Assert.ThrowsAsync<DaraApiException>(() => phone.Api.RegisterPluginsAsync([Camera with { Sha256 = "not-a-hash" }], cancellationToken));

        Assert.Equal((HttpStatusCode.BadRequest, "rule_broken", "PluginHashMustBeSha256Rule"), (error.StatusCode, error.Code, error.Rule));
        Assert.Contains("Plugin hash must be a sha-256", error.Detail);
        Assert.Empty(await phone.Api.GetCurrentClientPluginsAsync(cancellationToken));
    }
}
