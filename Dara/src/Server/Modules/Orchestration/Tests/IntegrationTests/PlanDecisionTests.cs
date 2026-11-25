using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;
using Dara.Server.Modules.Orchestration.Integration;
using static Dara.Server.Modules.Orchestration.IntegrationTests.Fakes.ScriptedPlanningChatClient;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

public class PlanDecisionTests
{
    [Fact]
    public async Task AuthorApprovesTheWholePlan()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Calls(("BobLaptop_TakePhoto", "{}")));
        var scenario = await PlanningScenario.CreateAsync(host);
        var runId = await CreatedPlanAsync(host, scenario);

        await host.ExecuteAsync(new ApproveExecutionPlanCommand(runId, scenario.PhoneId));

        var approved = await host.WaitForPublishedAsync<ExecutionPlanApprovedIntegrationEvent>(e => e.AgentRunId == runId, "approved plan");
        Assert.Equal(scenario.PhoneId, approved.AuthorId);
        Assert.Equal(scenario.GroupId, approved.GroupId);
        Assert.Equal("Approved", (await GetRunAsync(host, runId, scenario.PhoneId)).Status);
    }

    [Fact]
    public async Task AuthorRejectsTheWholePlan()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Calls(("BobLaptop_TakePhoto", "{}")));
        var scenario = await PlanningScenario.CreateAsync(host);
        var runId = await CreatedPlanAsync(host, scenario);

        await host.ExecuteAsync(new RejectExecutionPlanCommand(runId, scenario.PhoneId));

        await host.WaitForPublishedAsync<AgentRunRejectedIntegrationEvent>(e => e.AgentRunId == runId, "rejected plan");
        Assert.Equal("Rejected", (await GetRunAsync(host, runId, scenario.PhoneId)).Status);
    }

    [Fact]
    public async Task OnlyTheAuthorDecidesAndOnlyOnce()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Calls(("BobLaptop_TakePhoto", "{}")));
        var scenario = await PlanningScenario.CreateAsync(host);
        var runId = await CreatedPlanAsync(host, scenario);

        await AssertBrokenAsync<OnlyAuthorCanDecideAboutPlanRule>(() => host.ExecuteAsync(new ApproveExecutionPlanCommand(runId, scenario.LaptopId)));
        await AssertBrokenAsync<OnlyAuthorCanDecideAboutPlanRule>(() => host.ExecuteAsync(new RejectExecutionPlanCommand(runId, scenario.StrangerId)));
        Assert.Equal("AwaitingApproval", (await GetRunAsync(host, runId, scenario.PhoneId)).Status);

        await host.ExecuteAsync(new ApproveExecutionPlanCommand(runId, scenario.PhoneId));

        await AssertBrokenAsync<AgentRunMustBeInStatusRule>(() => host.ExecuteAsync(new RejectExecutionPlanCommand(runId, scenario.PhoneId)));
        Assert.Equal("Approved", (await GetRunAsync(host, runId, scenario.PhoneId)).Status);
    }

    [Fact]
    public async Task FailedGenerationHasNoPlanToApprove()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken, Text("no plan"));
        var scenario = await PlanningScenario.CreateAsync(host);
        var messageId = Guid.NewGuid();
        await host.ExecuteAsync(new StartAgentRunCommand(messageId, scenario.GroupId, scenario.PhoneId, "/prompt take a photo"));
        var started = await host.WaitForPublishedAsync<AgentRunStartedIntegrationEvent>(e => e.MessageId == messageId, "started run");
        await host.WaitForPublishedAsync<ExecutionPlanGenerationFailedIntegrationEvent>(e => e.AgentRunId == started.AgentRunId, "failed plan");

        await AssertBrokenAsync<AgentRunMustBeInStatusRule>(() => host.ExecuteAsync(new ApproveExecutionPlanCommand(started.AgentRunId, scenario.PhoneId)));
    }

    private static async Task<Guid> CreatedPlanAsync(OrchestrationModuleHost host, PlanningScenario scenario)
    {
        var messageId = Guid.NewGuid();
        await host.ExecuteAsync(new StartAgentRunCommand(messageId, scenario.GroupId, scenario.PhoneId, "/prompt take a photo on my laptop"));
        var started = await host.WaitForPublishedAsync<AgentRunStartedIntegrationEvent>(e => e.MessageId == messageId, "started run");
        await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == started.AgentRunId, "created plan");

        return started.AgentRunId;
    }

    private static async Task<AgentRunDto> GetRunAsync(OrchestrationModuleHost host, Guid runId, Guid authorId)
    {
        return (await host.QueryAsync<GetAgentRunQuery, AgentRunDto?>(new GetAgentRunQuery(runId, authorId)))!;
    }

    private static async Task AssertBrokenAsync<TRule>(Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
    }
}
