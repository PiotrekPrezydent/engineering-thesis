using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Shared.Contracts.Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using static Dara.Server.Modules.Orchestration.IntegrationTests.Fakes.ScriptedPlanningChatClient;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

public class GenerateExecutionPlanTests
{
    [Fact]
    public async Task PlanCallsPointToParticipantFunctionsAndUseResultsOfEarlierCalls()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobLaptop_TakePhoto", "{}"), ("BobPhone_GetPhotosFolder", "{}")),
            Calls(("BobPhone_SaveFile", """{"path":"{{result:2}}/photo.jpg","content":"{{result:1}}"}""")),
            Text("Photo from the laptop saved on the phone."));
        var scenario = await PlanningScenario.CreateAsync(host);

        var runId = await StartRunAsync(host, scenario);
        var created = await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == runId, "created plan");

        Assert.Equal(scenario.PhoneId, created.AuthorId);
        Assert.Equal([1, 2, 3], created.Calls.Select(c => c.Index));
        Assert.Equal([scenario.LaptopId, scenario.PhoneId, scenario.PhoneId], created.Calls.Select(c => c.ParticipantId));
        Assert.Equal(
            [scenario.FunctionId("TakePhoto"), scenario.FunctionId("GetPhotosFolder"), scenario.FunctionId("SaveFile")],
            created.Calls.Select(c => c.FunctionId));
        Assert.Equal(["Bob-Laptop", "Bob-Phone", "Bob-Phone"], created.Calls.Select(c => c.ParticipantName));
        Assert.Equal("""{"path":"{{result:2}}/photo.jpg","content":"{{result:1}}"}""", created.Calls[2].ArgumentsJson);

        var run = Assert.Single(await scenario.ReadRunsAsync(host));
        var save = run.Plan.Calls[2];
        Assert.Equal(CallArgumentKind.Text, save.Arguments.Single(a => a.Name == "path").Kind);
        Assert.Equal(CallArgumentKind.Result, save.Arguments.Single(a => a.Name == "content").Kind);
        Assert.Equal([1], save.Arguments.Single(a => a.Name == "content").ResultIndexes);

        // the model got placeholders as results, numbered like the plan
        var results = host.Model.Client.Requests[2].Messages.SelectMany(m => m.Contents).OfType<FunctionResultContent>();
        Assert.Equal(["{{result:1}}", "{{result:2}}", "{{result:3}}"], results.Select(r => r.Result?.ToString()));

        var dto = await host.QueryAsync<GetAgentRunQuery, AgentRunDto?>(new GetAgentRunQuery(runId, scenario.PhoneId));
        Assert.Equal("AwaitingApproval", dto!.Status);
        Assert.Equal(["BobLaptop_TakePhoto", "BobPhone_GetPhotosFolder", "BobPhone_SaveFile"], dto.Calls.Select(c => c.Alias));
        Assert.Null(await host.QueryAsync<GetAgentRunQuery, AgentRunDto?>(new GetAgentRunQuery(runId, scenario.LaptopId)));
    }

    [Fact]
    public async Task ModelSeesToolsUnderAliasesWithTheSchemasOfTheContract()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobLaptop_TakePhoto", """{"quality":90}""")));
        var scenario = await PlanningScenario.CreateAsync(host);

        var runId = await StartRunAsync(host, scenario, "/prompt take a photo on my laptop");
        await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == runId, "created plan");

        var request = host.Model.Client.Requests[0];
        var tools = request.Tools.Cast<AIFunctionDeclaration>().OrderBy(t => t.Name).ToList();
        Assert.Equal(["BobLaptop_TakePhoto", "BobPhone_GetPhotosFolder", "BobPhone_SaveFile"], tools.Select(t => t.Name));
        AssertSchema(PlanningScenario.TakePhoto, tools[0]);
        AssertSchema(PlanningScenario.GetPhotosFolder, tools[1]);
        AssertSchema(PlanningScenario.SaveFile, tools[2]);
        Assert.Contains("Bob-Laptop", tools[0].Description);

        var system = request.Messages.Single(m => m.Role == ChatRole.System).Text;
        Assert.Contains("- Bob-Phone: BobPhone_GetPhotosFolder, BobPhone_SaveFile", system);
        Assert.Contains("- Bob-Laptop: BobLaptop_TakePhoto", system);
        Assert.Contains("{{result:N}}", system);
        Assert.Equal("Bob-Phone : take a photo on my laptop", request.Messages.Single(m => m.Role == ChatRole.User).Text);

        Assert.Equal([(PlanningScenario.ApiKey, PlanningScenario.ModelName)], host.Model.CreatedClients);
    }

    [Fact]
    public async Task UnknownToolEndsAsAFailureWithTheReason()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobWatch_TakePhoto", "{}")),
            Text("I could not plan it."));
        var scenario = await PlanningScenario.CreateAsync(host);

        var failed = await FailedRunAsync(host, scenario);

        Assert.Contains("BobWatch_TakePhoto", failed.Reason);
        Assert.Equal("PlanGenerationFailed", Assert.Single(await scenario.ReadRunsAsync(host)).Status.Value);
    }

    [Fact]
    public async Task WrongArgumentEndsAsAFailureWithTheRuleMessage()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobPhone_SaveFile", """{"path":"photo.jpg","content":"x"}""")),
            Text("Done."));
        var scenario = await PlanningScenario.CreateAsync(host);

        var failed = await FailedRunAsync(host, scenario);

        Assert.Contains("Arguments of SaveFile are not valid", failed.Reason);
        Assert.Empty(Assert.Single(await scenario.ReadRunsAsync(host)).Plan.Calls);
    }

    [Fact]
    public async Task ModelFixesARejectedCall()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobPhone_SaveFile", """{"path":"photo.jpg","content":"x"}""")),
            Calls(("BobPhone_SaveFile", """{"path":"/photo.jpg","content":"x"}""")),
            Text("Saved."));
        var scenario = await PlanningScenario.CreateAsync(host);

        var runId = await StartRunAsync(host, scenario);
        var created = await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == runId, "created plan");

        Assert.Equal(1, Assert.Single(created.Calls).Index);
        var error = host.Model.Client.Requests[1].Messages.SelectMany(m => m.Contents).OfType<FunctionResultContent>().Single();
        Assert.StartsWith("Error: Arguments of SaveFile are not valid", error.Result?.ToString());
    }

    [Fact]
    public async Task TextAnswerWithoutCallsEndsAsAFailure()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Text("I have no tool for that."));
        var scenario = await PlanningScenario.CreateAsync(host);

        var failed = await FailedRunAsync(host, scenario);

        Assert.Equal("the model answered without a plan: I have no tool for that.", failed.Reason);
    }

    [Fact]
    public async Task ProviderExceptionIsAFailureAndTheModelIsNotCalledAgain()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Throws(new HttpRequestException($"401 for key {PlanningScenario.ApiKey}")));
        var scenario = await PlanningScenario.CreateAsync(host);

        var failed = await FailedRunAsync(host, scenario);

        Assert.Contains("the model provider failed: 401 for key ***", failed.Reason);
        Assert.DoesNotContain(PlanningScenario.ApiKey, failed.Reason);

        // a second run goes through the same outbox loop, so the first one was not retried in between
        host.Model.Script(Calls(("BobLaptop_TakePhoto", "{}")));
        var secondId = await StartRunAsync(host, scenario);
        await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == secondId, "second plan");
        Assert.Equal(2, host.Model.CreatedClients.Count);
    }

    [Fact]
    public async Task RepeatedGenerationDoesNotCallTheModel()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobLaptop_TakePhoto", "{}")));
        var scenario = await PlanningScenario.CreateAsync(host);
        var runId = await StartRunAsync(host, scenario);
        await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == runId, "created plan");

        await host.ExecuteAsync(new GenerateExecutionPlanCommand(runId));

        Assert.Single(host.Model.CreatedClients);
        Assert.Single(Assert.Single(await scenario.ReadRunsAsync(host)).Plan.Calls);
    }

    [Fact]
    public async Task MissingKeyOrUnknownProviderFailsWithoutCallingTheModel()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var scenario = await PlanningScenario.CreateAsync(host, withSettings: false);
        await host.ExecuteAsync(new EditChatClientProviderNameCommand(scenario.GroupId, scenario.PhoneId, "google"));

        var noKey = await FailedRunAsync(host, scenario);
        Assert.Equal("the group has no api key", noKey.Reason);

        await host.ExecuteAsync(new EditApiKeyCommand(scenario.GroupId, scenario.PhoneId, PlanningScenario.ApiKey));
        await host.ExecuteAsync(new EditChatClientProviderNameCommand(scenario.GroupId, scenario.PhoneId, "openai"));

        var unknown = await FailedRunAsync(host, scenario);
        Assert.Equal("unknown chat client provider 'openai'", unknown.Reason);
        Assert.Empty(host.Model.CreatedClients);
    }

    [Fact]
    public async Task ApiKeyIsNeitherInEventsNorInTheOutbox()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken,
            Calls(("BobLaptop_TakePhoto", "{}")));
        var scenario = await PlanningScenario.CreateAsync(host);
        var createdId = await StartRunAsync(host, scenario);
        await host.WaitForPublishedAsync<ExecutionPlanCreatedIntegrationEvent>(e => e.AgentRunId == createdId, "created plan");
        host.Model.Script(Throws(new InvalidOperationException($"bad key {PlanningScenario.ApiKey}")));
        await FailedRunAsync(host, scenario);

        var events = host.Published.Select(e => JsonSerializer.Serialize(e, e.GetType()));
        var outbox = await host.ReadAsync(async readModel => await readModel.Query<OutboxMessage>().Select(m => m.Content).ToListAsync());
        var runs = (await scenario.ReadRunsAsync(host)).Select(r => JsonSerializer.Serialize(new { r.GroupSnapshot, r.FailureReason }));

        Assert.NotEmpty(outbox);
        Assert.All(events.Concat(outbox).Concat(runs), text => Assert.DoesNotContain(PlanningScenario.ApiKey, text));
    }

    private static async Task<Guid> StartRunAsync(OrchestrationModuleHost host, PlanningScenario scenario, string message = "/prompt take a photo on my laptop and save it on my phone")
    {
        var messageId = Guid.NewGuid();
        await host.ExecuteAsync(new StartAgentRunCommand(messageId, scenario.GroupId, scenario.PhoneId, message));

        var started = await host.WaitForPublishedAsync<AgentRunStartedIntegrationEvent>(e => e.MessageId == messageId, "started run");
        return started.AgentRunId;
    }

    private static async Task<ExecutionPlanGenerationFailedIntegrationEvent> FailedRunAsync(OrchestrationModuleHost host, PlanningScenario scenario)
    {
        var runId = await StartRunAsync(host, scenario);
        return await host.WaitForPublishedAsync<ExecutionPlanGenerationFailedIntegrationEvent>(e => e.AgentRunId == runId, "failed plan");
    }

    private static void AssertSchema(FunctionDescription function, AIFunctionDeclaration tool)
    {
        Assert.Equal(FunctionJsonSchema.CreateParametersSchema(function).GetRawText(), tool.JsonSchema.GetRawText());
        Assert.Equal(FunctionJsonSchema.CreateReturnSchema(function)?.GetRawText(), tool.ReturnJsonSchema?.GetRawText());
    }
}
