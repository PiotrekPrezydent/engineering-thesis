using System.Text.Json;
using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.Modules.Identity.Integration;
using Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.SystemTests.Flows;

// plugin functions registered and configured in plugins reach the participant in orchestration
public class PluginFunctionsFlowTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { Converters = { new JsonStringEnumConverter() } };

    private static readonly FunctionDescription SaveFile = new("SaveFile", "Saves content as a file",
    [
        new PropertyDescription("path", new TypeDescription(TypeKind.String, Description: "Where the file is saved"), true),
        new PropertyDescription("tags", new TypeDescription(TypeKind.Array, Nullable: true, Items: new TypeDescription(TypeKind.String, Enum: ["work", "home"])), false)
    ], new TypeDescription(TypeKind.Boolean));

    private static readonly FunctionDescription ReadFile = new("ReadFile", "Reads a file",
        [new PropertyDescription("path", new TypeDescription(TypeKind.String), true)], new TypeDescription(TypeKind.String));

    private static readonly FunctionDescription TakePhoto = new("TakePhoto", null, [], new TypeDescription(TypeKind.String, ContentEncoding: "base64"));

    private static PluginManifest Files => new("Files", null, "1.0.0", new string('a', 64), [SaveFile, ReadFile]);

    private static PluginManifest Camera => new("Camera", null, "1.0.0", new string('c', 64), [TakePhoto]);

    [Fact]
    public async Task RegisteredFunctionsReachTheParticipantWithFullDescription()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var clientId = await CreateClientAsync(host, "plugins-user");

        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(clientId, [Files, Camera]));

        var pluginFunctionIds = (await GetPluginsAsync(host, clientId)).SelectMany(p => p.Functions).Select(f => f.FunctionId).Order().ToList();
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "SaveFile", "TakePhoto"]);

        var functions = await GetFunctionsAsync(host, clientId);
        Assert.Equal(pluginFunctionIds, functions.Select(f => f.FunctionId).Order());

        var saveFile = await ModulesHost.ReadAsync(host.OrchestrationRoot, model => model.Query<Function>()
            .SingleAsync(f => f.ParticipantId.Match(clientId) && f.Name == "SaveFile"));
        Assert.Equal(JsonSerializer.Serialize(SaveFile, JsonOptions), JsonSerializer.Serialize(saveFile.GetDescription(), JsonOptions));
    }

    [Fact]
    public async Task DisabledFunctionDisappearsAndEnabledComesBack()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var clientId = await CreateClientAsync(host, "configuring-user");

        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(clientId, [Files, Camera]));
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "SaveFile", "TakePhoto"]);

        var files = (await GetPluginsAsync(host, clientId)).Single(p => p.Name == "Files");
        var saveFile = files.Functions.Single(f => f.Description.Name == "SaveFile");

        await host.Plugins.ExecuteCommandAsync(new DisablePluginFunctionCommand(clientId, files.PluginId, saveFile.FunctionId));
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "TakePhoto"]);

        await host.Plugins.ExecuteCommandAsync(new EnablePluginFunctionCommand(clientId, files.PluginId, saveFile.FunctionId));
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "SaveFile", "TakePhoto"]);

        await host.Plugins.ExecuteCommandAsync(new DisablePluginCommand(clientId, files.PluginId));
        await WaitForFunctionsAsync(host, clientId, ["TakePhoto"]);

        await host.Plugins.ExecuteCommandAsync(new EnablePluginCommand(clientId, files.PluginId));
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "SaveFile", "TakePhoto"]);
    }

    [Fact]
    public async Task PluginMissingFromTheNextRegistrationLeavesTheParticipant()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var clientId = await CreateClientAsync(host, "reconnecting-user");

        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(clientId, [Files, Camera]));
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "SaveFile", "TakePhoto"]);

        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(clientId, [Camera]));
        await WaitForFunctionsAsync(host, clientId, ["TakePhoto"]);

        // a new build of the plugin has a new hash and new function ids
        var oldIds = (await GetFunctionsAsync(host, clientId)).Select(f => f.FunctionId).ToList();
        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(clientId, [Camera with { Sha256 = new string('d', 64) }]));
        await ModulesHost.WaitUntilAsync(async () =>
        {
            var functions = await GetFunctionsAsync(host, clientId);
            return functions.Count == 1 && !oldIds.Contains(functions[0].FunctionId);
        }, "functions of the new camera build");
    }

    [Fact]
    public async Task PluginsRegisteredBeforeTheUserIsProjectedReachTheParticipant()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var clientId = Guid.NewGuid();

        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(clientId, [Files]));
        await WaitForFunctionsAsync(host, clientId, ["ReadFile", "SaveFile"]);

        // the client event comes late to both modules and must not fail there
        var userCreated = new UserClientRegisteredIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, clientId, Guid.NewGuid(), "PC 1", "Pc");
        await host.EventBus.PublishAsync(userCreated);

        await ModulesHost.WaitUntilAsync(() => ProcessedAsync(host.PluginsRoot, userCreated.EventId), "new user event processed in plugins");
        await ModulesHost.WaitUntilAsync(() => ProcessedAsync(host.OrchestrationRoot, userCreated.EventId), "new user event processed in orchestration");

        Assert.Equal(1, await ModulesHost.ReadAsync(host.PluginsRoot, model => model.Query<PluginOwner>().CountAsync(o => o.Id.Match(clientId))));
        Assert.Equal(1, await ModulesHost.ReadAsync(host.OrchestrationRoot, model => model.Query<Participant>().CountAsync(p => p.Id.Match(clientId))));
        Assert.Equal(["ReadFile", "SaveFile"], (await GetFunctionsAsync(host, clientId)).Select(f => f.Name));
    }

    private static async Task<Guid> CreateClientAsync(ModulesHost host, string identifier)
    {
        var clientId = (await host.CreateClientAsync(identifier)).ClientId;

        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(host.PluginsRoot,
            model => model.Query<PluginOwner>().AnyAsync(o => o.Id.Match(clientId))), "plugin owner of the new client");
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(host.OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(clientId))), "participant of the new client");

        return clientId;
    }

    private static Task<List<OwnerPluginDto>> GetPluginsAsync(ModulesHost host, Guid clientId)
        => host.Plugins.ExecuteQueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(clientId));

    private static Task<List<ParticipantFunctionDto>> GetFunctionsAsync(ModulesHost host, Guid clientId)
        => host.Orchestration.ExecuteQueryAsync<GetParticipantFunctionsQuery, List<ParticipantFunctionDto>>(new GetParticipantFunctionsQuery(clientId));

    private static Task WaitForFunctionsAsync(ModulesHost host, Guid clientId, IReadOnlyList<string> names)
        => ModulesHost.WaitUntilAsync(async () => (await GetFunctionsAsync(host, clientId)).Select(f => f.Name).SequenceEqual(names),
            $"participant functions {string.Join(", ", names)}");

    private static Task<bool> ProcessedAsync(IModuleCompositionRoot root, Guid eventId)
        => ModulesHost.ReadAsync(root, model => model.Query<InboxMessage>().AnyAsync(m => m.Id == eventId && m.ProcessedDate != null));
}
