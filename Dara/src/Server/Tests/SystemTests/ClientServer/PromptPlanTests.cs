using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.Plugins;
using Microsoft.AspNetCore.SignalR;
using static Dara.Server.Modules.Orchestration.IntegrationTests.Fakes.ScriptedPlanningChatClient;

namespace Dara.Server.SystemTests.ClientServer;

// a prompt sent as a group message over the hub, its plan goes only to the author, who approves or rejects it
public class PromptPlanTests
{
    private const string ApiKey = "client-server-test-key-not-real";
    private const string Prompt = "/prompt take a photo on my laptop and save it on my phone";

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
    public async Task PlanGoesOnlyToTheAuthorWhoApprovesIt()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var (phone, laptop, groupId) = await CreateDevicesAsync(host, cancellationToken);
        host.Model.Script(
            Calls(("BobLaptop_TakePhoto", "{}")),
            Calls(("BobPhone_SaveFile", """{"path":"/photos/new.jpg","content":"{{result:1}}"}""")),
            Text("Photo taken on the laptop and saved on the phone."));

        await phone.Hub.SendGroupMessage(groupId, Prompt);

        var started = await phone.Events.WaitForAsync<AgentRunStartedNotification>(n => n.GroupId == groupId, "agent run started");
        var plan = await phone.Events.WaitForAsync<ExecutionPlanCreatedNotification>(n => n.AgentRunId == started.AgentRunId, "plan at the author");
        var promptMessage = await phone.Events.WaitForAsync<GroupMessageReceivedNotification>(m => m.Message == Prompt, "prompt in the group");
        Assert.Equal(promptMessage.MessageId, started.MessageId);
        Assert.Equal([laptop.UserId, phone.UserId], plan.Calls.Select(c => c.ParticipantId));
        Assert.Equal(["TakePhoto", "SaveFile"], plan.Calls.Select(c => c.FunctionName));
        Assert.Equal("""{"path":"/photos/new.jpg","content":"{{result:1}}"}""", plan.Calls[1].ArgumentsJson);
        Assert.Equal([(ApiKey, "gemini-test")], host.Model.CreatedClients);

        await AssertNoAgentRunNotificationsAsync(laptop, groupId);
        Assert.Null(await laptop.Hub.GetAgentRun(started.AgentRunId));
        var run = await phone.Hub.GetAgentRun(started.AgentRunId);
        Assert.Equal("AwaitingApproval", run!.Status);
        Assert.Equal(2, run.Calls.Count);

        var error = await Assert.ThrowsAsync<HubException>(() => laptop.Hub.ApproveExecutionPlan(started.AgentRunId));
        Assert.Contains("Only the author of the prompt can approve or reject its plan", error.Message);

        await phone.Hub.ApproveExecutionPlan(started.AgentRunId);

        await phone.Events.WaitForAsync<ExecutionPlanApprovedNotification>(n => n.AgentRunId == started.AgentRunId, "plan approved");
        Assert.Equal("Approved", (await phone.Hub.GetAgentRun(started.AgentRunId))!.Status);
        await AssertNoAgentRunNotificationsAsync(laptop, groupId);
    }

    [Fact]
    public async Task AuthorRejectsThePlanAndCannotApproveItLater()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var (phone, _, groupId) = await CreateDevicesAsync(host, cancellationToken);
        host.Model.Script(Calls(("BobLaptop_TakePhoto", "{}")), Text("Photo taken."));

        await phone.Hub.SendGroupMessage(groupId, "/prompt take a photo on my laptop");
        var plan = await phone.Events.WaitForAsync<ExecutionPlanCreatedNotification>(n => n.GroupId == groupId, "plan at the author");

        await phone.Hub.RejectExecutionPlan(plan.AgentRunId);

        await phone.Events.WaitForAsync<AgentRunRejectedNotification>(n => n.AgentRunId == plan.AgentRunId, "run rejected");
        var error = await Assert.ThrowsAsync<HubException>(() => phone.Hub.ApproveExecutionPlan(plan.AgentRunId));
        Assert.Contains("Cannot approve", error.Message);
        Assert.Equal("Rejected", (await phone.Hub.GetAgentRun(plan.AgentRunId))!.Status);
    }

    [Fact]
    public async Task AnswerWithoutCallsIsAFailureForTheAuthor()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var (phone, laptop, groupId) = await CreateDevicesAsync(host, cancellationToken);
        host.Model.Script(Text("I cannot do that."));

        await laptop.Hub.SendGroupMessage(groupId, "/prompt make me a coffee");

        var failed = await laptop.Events.WaitForAsync<ExecutionPlanGenerationFailedNotification>(n => n.GroupId == groupId, "failure at the author");
        Assert.False(string.IsNullOrWhiteSpace(failed.Reason));
        Assert.Equal("PlanGenerationFailed", (await laptop.Hub.GetAgentRun(failed.AgentRunId))!.Status);
        await AssertNoAgentRunNotificationsAsync(phone, groupId);
    }

    // bob has a phone with files (the group owner) and a laptop with a camera, the owner sets the scripted provider
    private static async Task<(HubTestClient Phone, HubTestClient Laptop, Guid GroupId)> CreateDevicesAsync(AppHubTestHost host, CancellationToken cancellationToken)
    {
        var phone = await host.ConnectAsync("bob-phone", cancellationToken);
        var laptop = await host.ConnectAsync("bob-laptop", cancellationToken);

        await phone.Hub.ChangeName("Bob-Phone");
        await laptop.Hub.ChangeName("Bob-Laptop");
        await phone.Hub.RegisterPlugins([Files]);
        await laptop.Hub.RegisterPlugins([Camera]);
        await host.WaitForParticipantAsync(phone.UserId, "Bob-Phone", "SaveFile");
        await host.WaitForParticipantAsync(laptop.UserId, "Bob-Laptop", "TakePhoto");

        var group = await host.CreateGroupAsync(phone, "bob-devices", laptop);
        await phone.Hub.EditGroupChatClientProviderName(group.GroupId, "google");
        await phone.Hub.EditGroupClientModelName(group.GroupId, "gemini-test");
        await phone.Hub.EditGroupApiKey(group.GroupId, ApiKey);

        return (phone, laptop, group.GroupId);
    }

    // a message of the device after the plan arrives after every earlier notification for that device
    private static async Task AssertNoAgentRunNotificationsAsync(HubTestClient device, Guid groupId)
    {
        var marker = $"marker-{Guid.NewGuid():N}";
        await device.Hub.SendGroupMessage(groupId, marker);
        await device.Events.WaitForAsync<GroupMessageReceivedNotification>(m => m.Message == marker, $"marker at {device.Identifier}");

        Assert.Empty(device.Events.Received<AgentRunStartedNotification>());
        Assert.Empty(device.Events.Received<ExecutionPlanCreatedNotification>());
        Assert.Empty(device.Events.Received<ExecutionPlanGenerationFailedNotification>());
        Assert.Empty(device.Events.Received<ExecutionPlanApprovedNotification>());
        Assert.Empty(device.Events.Received<AgentRunRejectedNotification>());
    }
}
