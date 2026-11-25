using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.Plugins;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.SystemTests.ClientServer;

// plugins registered over the hub, the owner gets its registered set back
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

        await phone.Hub.RegisterPlugins([Camera]);
        await laptop.Hub.RegisterPlugins([Files, Notes]);

        await phone.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 1, "camera registered");
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 2, "files and notes registered");

        var phonePlugins = await phone.Hub.GetMyPlugins();
        var laptopPlugins = await laptop.Hub.GetMyPlugins();
        Assert.Equal(["Camera"], phonePlugins.Select(p => p.Name));
        Assert.Equal(["Files", "Notes"], laptopPlugins.Select(p => p.Name));
        Assert.Equal(["DeleteFile", "SaveFile"], laptopPlugins[0].Functions.Select(f => f.Description.Name));
        Assert.All(laptopPlugins.SelectMany(p => p.Functions), f => Assert.True(f.IsEnabled));
        Assert.DoesNotContain(laptop.Events.Received<PluginsRegisteredNotification>(), n => n.Plugins.Any(p => p.Name == "Camera"));

        // the cli sends its whole set again after every connection
        await phone.ReconnectAsync(cancellationToken);
        await phone.Hub.RegisterPlugins([Camera]);

        var afterReconnection = await phone.Hub.GetMyPlugins();
        var camera = Assert.Single(afterReconnection);
        Assert.Equal(phonePlugins[0].PluginId, camera.PluginId);
    }

    [Fact]
    public async Task NewSetReplacesTheRegisteredOne()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var laptop = await host.ConnectAsync("replacing-laptop", cancellationToken);

        await laptop.Hub.RegisterPlugins([Files, Notes]);
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 2, "files and notes registered");
        var notesBefore = (await laptop.Hub.GetMyPlugins()).Single(p => p.Name == "Notes");

        var newNotes = Notes with { Version = "2.2.0", Sha256 = new string('b', 64) };
        await laptop.Hub.RegisterPlugins([newNotes]);

        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 1 && n.Plugins[0].Version == "2.2.0", "only the new notes");
        var notesAfter = Assert.Single(await laptop.Hub.GetMyPlugins());
        Assert.NotEqual(notesBefore.PluginId, notesAfter.PluginId);
    }

    [Fact]
    public async Task OwnerDisablesAndEnablesAFunctionAndOthersCannot()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var laptop = await host.ConnectAsync("owner-laptop", cancellationToken);
        var stranger = await host.ConnectAsync("stranger-phone", cancellationToken);

        await laptop.Hub.RegisterPlugins([Files]);
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(n => n.Plugins.Count == 1, "files registered");
        var files = Assert.Single(await laptop.Hub.GetMyPlugins());
        var deleteFile = files.Functions.Single(f => f.Description.Name == "DeleteFile");

        var error = await Assert.ThrowsAsync<HubException>(() => stranger.Hub.DisablePluginFunction(files.PluginId, deleteFile.FunctionId));
        Assert.Contains("Only the plugin owner can change its configuration", error.Message);

        await laptop.Hub.DisablePluginFunction(files.PluginId, deleteFile.FunctionId);
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(
            n => n.Plugins.Single().Functions.Any(f => f.FunctionId == deleteFile.FunctionId && !f.IsEnabled), "delete file disabled");
        await ModulesHost.WaitUntilAsync(async () => (await host.GetParticipantFunctionNamesAsync(laptop.UserId)).SequenceEqual(["SaveFile"]),
            "only save file left in orchestration");

        await laptop.Hub.EnablePluginFunction(files.PluginId, deleteFile.FunctionId);
        await laptop.Events.WaitForAsync<PluginsRegisteredNotification>(
            n => n.Plugins.Single().Functions.All(f => f.IsEnabled), "delete file enabled again");
        await ModulesHost.WaitUntilAsync(async () => (await host.GetParticipantFunctionNamesAsync(laptop.UserId)).SequenceEqual(["DeleteFile", "SaveFile"]),
            "delete file back in orchestration");
    }

    [Fact]
    public async Task BrokenRuleOfARegistrationIsAnErrorForTheCaller()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var phone = await host.ConnectAsync("bad-plugin-phone", cancellationToken);

        var error = await Assert.ThrowsAsync<HubException>(() => phone.Hub.RegisterPlugins([Camera with { Sha256 = "not-a-hash" }]));

        Assert.Contains("Plugin hash must be a sha-256", error.Message);
        Assert.Empty(await phone.Hub.GetMyPlugins());
    }
}
