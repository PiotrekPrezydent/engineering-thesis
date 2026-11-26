using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;
using Dara.Server.Modules.Orchestration.Integration;
using Microsoft.EntityFrameworkCore;
using static Dara.Server.Modules.Orchestration.IntegrationTests.Fakes.ScriptedPlanningChatClient;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

public class LongInternalCommandTests
{
    [Fact]
    public async Task HeldPlanGenerationDoesNotHoldBackTheOutboxOfTheModule()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            WaitsFor(gate.Task, Calls(("BobLaptop_TakePhoto", "{}"))));
        var first = await PlanningScenario.CreateAsync(host);
        var second = await PlanningScenario.CreateAsync(host);
        var firstRunId = Guid.Empty;

        try
        {
            firstRunId = await StartRunAsync(host, first);
            await OrchestrationModuleHost.WaitUntilAsync(() => Task.FromResult(host.Model.Client.Requests.Count == 1), "first generation waits for the model");

            // the outbox publishes the start of another run while the first generation is still running
            var secondRunId = await StartRunAsync(host, second);
            Assert.NotEqual(firstRunId, secondRunId);
            Assert.False(await IsGenerationProcessedAsync(host, firstRunId));
            Assert.DoesNotContain(host.Published.OfType<ExecutionPlanCreatedIntegrationEvent>(), e => e.AgentRunId == firstRunId);
        }
        finally
        {
            gate.TrySetResult();
        }

        await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == firstRunId, "plan of the first run after the model answers");
    }

    private static async Task<Guid> StartRunAsync(OrchestrationModuleHost host, PlanningScenario scenario)
    {
        var messageId = Guid.NewGuid();
        await host.ExecuteAsync(new StartAgentRunCommand(messageId, scenario.GroupId, scenario.PhoneId, "/prompt take a photo on my laptop"));

        var started = await host.WaitForPublishedAsync<AgentRunStartedIntegrationEvent>(e => e.MessageId == messageId, "started run");
        return started.AgentRunId;
    }

    private static Task<bool> IsGenerationProcessedAsync(OrchestrationModuleHost host, Guid runId) =>
        host.ReadAsync(read => read.Query<InternalCommand>()
            .AnyAsync(c => c.Type == "GenerateExecutionPlanCommand" && c.Data.Contains(runId.ToString()) && c.ProcessedDate != null));
}
