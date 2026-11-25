using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;
using Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;
using Dara.Server.Modules.Orchestration.Domain.Groups.Rules;
using Dara.Server.Modules.Orchestration.Integration;
using static Dara.Server.Modules.Orchestration.IntegrationTests.Fakes.ScriptedPlanningChatClient;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

public class StartAgentRunTests
{
    [Fact]
    public async Task MessageWithThePrefixStartsARunAndAnOrdinaryMessageDoesNot()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Text("nothing to do"));
        var scenario = await PlanningScenario.CreateAsync(host);
        var ordinaryId = Guid.NewGuid();
        var promptId = Guid.NewGuid();

        // both messages go through the inbox handler, only the prompt starts a run
        await host.EventBus.PublishAsync(Message(scenario, ordinaryId, "/promptless talk, not a prompt"));
        await host.EventBus.PublishAsync(Message(scenario, promptId, "/prompt   take a photo  "));

        var started = await host.WaitForPublishedAsync<AgentRunStartedIntegrationEvent>(e => e.MessageId == promptId, "run of the prompt");

        var run = Assert.Single(await scenario.ReadRunsAsync(host));
        Assert.Equal(started.AgentRunId, run.Id.Value);
        Assert.Equal(scenario.PhoneId, started.AuthorId);
        Assert.Equal(scenario.GroupId, started.GroupId);
        Assert.Equal("take a photo", run.Prompt);
        Assert.DoesNotContain(host.Published.OfType<AgentRunStartedIntegrationEvent>(), e => e.MessageId == ordinaryId);
    }

    [Fact]
    public async Task OwnerChangesThePrefixOfPrompts()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Text("nothing to do"));
        var scenario = await PlanningScenario.CreateAsync(host);
        await host.ExecuteAsync(new EditPromptMessagePrefixCommand(scenario.GroupId, scenario.PhoneId, "/ai"));

        await host.ExecuteAsync(new StartAgentRunCommand(Guid.NewGuid(), scenario.GroupId, scenario.LaptopId, "/prompt take a photo"));
        Assert.Empty(await scenario.ReadRunsAsync(host));

        await host.ExecuteAsync(new StartAgentRunCommand(Guid.NewGuid(), scenario.GroupId, scenario.LaptopId, "/ai take a photo"));
        var run = Assert.Single(await scenario.ReadRunsAsync(host));
        Assert.Equal(scenario.LaptopId, run.AuthorId.Value);
    }

    [Fact]
    public async Task RunKeepsOnlyParticipantsOfTheGroupAndNoApiKey()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Text("nothing to do"));
        var scenario = await PlanningScenario.CreateAsync(host);

        await host.ExecuteAsync(new StartAgentRunCommand(Guid.NewGuid(), scenario.GroupId, scenario.PhoneId, "/prompt take a photo"));

        var run = Assert.Single(await scenario.ReadRunsAsync(host));
        Assert.Equal(["Bob-Laptop", "Bob-Phone"], run.ParticipantsSnapshots.Select(p => p.Name).Order());
        Assert.Equal(["BobLaptop_TakePhoto", "BobPhone_GetPhotosFolder", "BobPhone_SaveFile"], run.ToolAliases.Select(a => a.Alias).Order());
        Assert.DoesNotContain(PlanningScenario.ApiKey, System.Text.Json.JsonSerializer.Serialize(run.GroupSnapshot));
    }

    [Fact]
    public async Task StrangerCannotStartARunInTheGroup()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var scenario = await PlanningScenario.CreateAsync(host);

        var exception = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync(new StartAgentRunCommand(Guid.NewGuid(), scenario.GroupId, scenario.StrangerId, "/prompt take a photo")));

        Assert.IsType<AuthorMustBeGroupParticipantRule>(exception.BrokenRule);
        Assert.Empty(await scenario.ReadRunsAsync(host));
    }

    private static GroupMessageAddedIntegrationEvent Message(PlanningScenario scenario, Guid messageId, string content)
    {
        return new GroupMessageAddedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, messageId, scenario.GroupId, scenario.PhoneId, content);
    }
}
