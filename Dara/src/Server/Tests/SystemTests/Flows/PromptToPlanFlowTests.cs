using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;
using Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;
using Dara.Server.Modules.Identity.Application.CreateUser;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Server.Modules.Orchestration.IntegrationTests.Fakes;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;
using Dara.Server.Modules.Profiles.Application.GetProfile;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Plugins;
using Microsoft.EntityFrameworkCore;
using static Dara.Server.Modules.Orchestration.IntegrationTests.Fakes.ScriptedPlanningChatClient;

namespace Dara.Server.SystemTests.Flows;

// a prompt sent to the group in groups becomes a plan in orchestration for the functions registered in plugins
public class PromptToPlanFlowTests
{
    private const string ApiKey = "system-test-key-not-real";

    private static PluginManifest Camera => new("Camera", null, "1.0.0", new string('c', 64),
        [new FunctionDescription("TakePhoto", "Takes a photo", [], new TypeDescription(TypeKind.String, ContentEncoding: "base64"))]);

    private static PluginManifest Files => new("Files", null, "1.0.0", new string('f', 64),
    [
        new FunctionDescription("SaveFile", "Saves a file",
        [
            new PropertyDescription("path", new TypeDescription(TypeKind.String), true),
            new PropertyDescription("content", new TypeDescription(TypeKind.String), true)
        ], new TypeDescription(TypeKind.Boolean))
    ]);

    [Fact]
    public async Task PromptInTheGroupBecomesAPlanForTheDevicesOfTheGroup()
    {
        var model = new ScriptedChatClientProvider(
            Calls(("BobLaptop_TakePhoto", "{}")),
            Calls(("BobPhone_SaveFile", """{"path":"/photos/new.jpg","content":"{{result:1}}"}""")),
            Text("Photo taken on the laptop and saved on the phone."));
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken, model);
        var plans = new PublishedPlans();
        host.EventBus.Subscribe<AgentRunStartedIntegrationEvent>(plans);
        host.EventBus.Subscribe<ExecutionPlanCreatedIntegrationEvent>(plans);

        var phoneId = await CreateDeviceAsync(host, "bob-phone", "Bob-Phone", Files, "SaveFile");
        var laptopId = await CreateDeviceAsync(host, "bob-laptop", "Bob-Laptop", Camera, "TakePhoto");

        var groupId = await host.Groups.ExecuteCommandAsync<CreateNewGroupCommand, Guid>(new CreateNewGroupCommand(phoneId, "bob-devices", "bob-jc"));
        await host.Groups.ExecuteCommandAsync(new JoinToGroupCommand(groupId, laptopId, "bob-jc"));
        await ModulesHost.WaitUntilAsync(async () => await GetSettingsAsync(host, groupId, laptopId) is not null, "laptop in the orchestration group");

        await host.Orchestration.ExecuteCommandAsync(new EditChatClientProviderNameCommand(groupId, phoneId, "google"));
        await host.Orchestration.ExecuteCommandAsync(new EditClientModelNameCommand(groupId, phoneId, "gemini-test"));
        await host.Orchestration.ExecuteCommandAsync(new EditApiKeyCommand(groupId, phoneId, ApiKey));

        await host.Groups.ExecuteCommandAsync(new SendGroupMessageCommand(groupId, laptopId, "hello, nothing to plan"));
        await host.Groups.ExecuteCommandAsync(new SendGroupMessageCommand(groupId, phoneId, "/prompt take a photo on my laptop and save it on my phone"));

        await ModulesHost.WaitUntilAsync(() => Task.FromResult(plans.Created.Count > 0), "plan of the prompt");

        var plan = Assert.Single(plans.Created);
        Assert.Equal(phoneId, plan.AuthorId);
        Assert.Equal(groupId, plan.GroupId);
        Assert.Equal([laptopId, phoneId], plan.Calls.Select(c => c.ParticipantId));
        Assert.Equal(["TakePhoto", "SaveFile"], plan.Calls.Select(c => c.FunctionName));
        Assert.Equal("""{"path":"/photos/new.jpg","content":"{{result:1}}"}""", plan.Calls[1].ArgumentsJson);

        Assert.Single(plans.Started);
        Assert.Equal([(ApiKey, "gemini-test")], model.CreatedClients);
        Assert.Equal("Bob-Phone : take a photo on my laptop and save it on my phone",
            model.Client.Requests[0].Messages.Last().Text);
    }

    private static async Task<Guid> CreateDeviceAsync(ModulesHost host, string identifier, string name, PluginManifest plugin, string functionName)
    {
        var userId = await host.Identity.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand(identifier));

        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(host.OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(userId))), "participant of the new user");
        await ModulesHost.WaitUntilAsync(async () =>
        {
            try
            {
                return await host.Profiles.ExecuteQueryAsync<GetProfileQuery, ProfileDto>(new GetProfileQuery(userId)) is not null;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }, "profile of the new user");

        await host.Profiles.ExecuteCommandAsync(new ChangeProfileNameCommand(userId, name));
        await host.Plugins.ExecuteCommandAsync(new RegisterPluginsCommand(userId, [plugin]));

        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(host.OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(userId) && p.Name == name)), "name of the participant");
        await ModulesHost.WaitUntilAsync(async () =>
            (await host.Orchestration.ExecuteQueryAsync<GetParticipantFunctionsQuery, List<ParticipantFunctionDto>>(new GetParticipantFunctionsQuery(userId)))
            .Any(f => f.Name == functionName), "functions of the participant");

        return userId;
    }

    private static Task<GroupAgentSettingsDto?> GetSettingsAsync(ModulesHost host, Guid groupId, Guid requesterId)
    {
        return host.Orchestration.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(groupId, requesterId));
    }

    // the outbox loop of orchestration publishes on its own thread
    private class PublishedPlans :
        IIntegrationEventHandler<AgentRunStartedIntegrationEvent>,
        IIntegrationEventHandler<ExecutionPlanCreatedIntegrationEvent>
    {
        private readonly Lock _lock = new();
        private readonly List<AgentRunStartedIntegrationEvent> _started = new();
        private readonly List<ExecutionPlanCreatedIntegrationEvent> _created = new();

        public IReadOnlyList<AgentRunStartedIntegrationEvent> Started
        {
            get
            {
                lock (_lock)
                    return _started.ToList();
            }
        }

        public IReadOnlyList<ExecutionPlanCreatedIntegrationEvent> Created
        {
            get
            {
                lock (_lock)
                    return _created.ToList();
            }
        }

        public Task HandleAsync(AgentRunStartedIntegrationEvent integrationEvent)
        {
            lock (_lock)
                _started.Add(integrationEvent);

            return Task.CompletedTask;
        }

        public Task HandleAsync(ExecutionPlanCreatedIntegrationEvent integrationEvent)
        {
            lock (_lock)
                _created.Add(integrationEvent);

            return Task.CompletedTask;
        }
    }
}
