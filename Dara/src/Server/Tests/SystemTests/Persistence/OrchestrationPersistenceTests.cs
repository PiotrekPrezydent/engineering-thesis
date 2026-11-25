using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;
using Dara.Server.SystemTests.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.SystemTests.Persistence;

// the orchestration ef model keeps descriptions, snapshots and plans, the run version stops stale saves
public class OrchestrationPersistenceTests
{
    private static TypeDescription Text => new(TypeKind.String);

    private static readonly FunctionDescription ReadFile = new("ReadFile", "Reads a file",
        [new("path", Text with { Pattern = "^/" }, true)], Text);

    private static readonly FunctionDescription SaveFile = new("SaveFile", null,
    [
        new("path", Text, true),
        new("content", Text, true),
        new("tags", new TypeDescription(TypeKind.Array, Nullable: true, Items: Text with { Enum = ["a", "b"] }), false)
    ], null);

    private static readonly FunctionDescription Ping = new("Ping", null, [], null);

    [Fact]
    public async Task GroupWithItsAgentSettingsIsStoredAndRead()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var owner = new ParticipantId(Guid.NewGuid());
        var member = new ParticipantId(Guid.NewGuid());
        var groupId = new GroupId(Guid.NewGuid());

        await InScope(host, async services =>
            await services.GetRequiredService<IGroupRepository>().AddAsync(Group.CreateDefault(groupId, owner)));

        await InScope(host, async services =>
        {
            var group = await services.GetRequiredService<IGroupRepository>().GetByIdAsync(groupId);
            group.AddParticipant(member);
            group.EditPromptMessagePrefix(owner, "!ai");
            group.EditApiKey(owner, "key-from-test");
        });

        await InScope(host, async services =>
        {
            var group = await services.GetRequiredService<IGroupRepository>().GetByIdAsync(groupId);
            Assert.Equal(owner, group.OwnerId);
            Assert.Equal(new[] { owner, member }, group.Participants);
            Assert.Equal(new GroupAgentSettings("!ai", "", ""), group.AgentSettings);
            Assert.Equal("key-from-test", group.ApiKey);
        });
    }

    [Fact]
    public async Task ParticipantFunctionsAreReplacedAndRead()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var participantId = new ParticipantId(Guid.NewGuid());
        var plugin = new PluginId(Guid.NewGuid());
        var readFile = new AvailableFunction(new FunctionId(Guid.NewGuid()), ReadFile);
        var saveFile = new AvailableFunction(new FunctionId(Guid.NewGuid()), SaveFile);
        var ping = new AvailableFunction(new FunctionId(Guid.NewGuid()), Ping);

        await InScope(host, async services =>
        {
            var participant = Participant.CreateDefault(participantId);
            participant.ReplacePluginFunctions(plugin, 1, [readFile, saveFile]);
            await services.GetRequiredService<IParticipantRepository>().AddAsync(participant);
        });

        // a function kept and one removed and one added in one save
        await InScope(host, async services =>
        {
            var participant = await services.GetRequiredService<IParticipantRepository>().GetByIdAsync(participantId);
            Assert.True(participant.ReplacePluginFunctions(plugin, 2, [saveFile, ping]));
        });

        await InScope(host, async services =>
        {
            var participant = await services.GetRequiredService<IParticipantRepository>().GetByIdAsync(participantId);
            Assert.Equal(new[] { "Ping", "SaveFile" }, participant.Functions.Select(f => f.Name).Order());
            Assert.Equal(Json(SaveFile), Json(participant.Functions.Single(f => f.Id == saveFile.Id).GetDescription()));
            Assert.Equal(new PluginVersion(plugin, 2), Assert.Single(participant.PluginVersions));
            Assert.False(participant.ReplacePluginFunctions(plugin, 1, []));
        });
    }

    [Fact]
    public async Task AgentRunWithItsPlanIsStoredAndRead()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var run = NewRun();
        run.ProposeCall(new ProposedCall("BobsPhone_ReadFile", """{"path":"/notes.txt"}"""));
        run.ProposeCall(new ProposedCall("BobsPhone_SaveFile", """{"path":"/copy.txt","content":"{{result:1}}","tags":["a"]}"""));

        await InScope(host, async services => await services.GetRequiredService<IAgentRunRepository>().AddAsync(run));

        await InScope(host, async services =>
        {
            var stored = await services.GetRequiredService<IAgentRunRepository>().GetByIdAsync(run.Id);
            Assert.Equal("PlanGenerating", stored.Status.Value);
            Assert.Equal(Json(run.GroupSnapshot), Json(stored.GroupSnapshot));
            Assert.Equal(Json(run.ParticipantsSnapshots), Json(stored.ParticipantsSnapshots));
            Assert.Equal(Json(run.ToolAliases), Json(stored.ToolAliases));
            Assert.Equal(Json(run.Plan.Calls), Json(stored.Plan.Calls));

            // a call added to a stored plan is saved too
            stored.ProposeCall(new ProposedCall("BobsPhone_Ping", "{}"));
            stored.CompletePlan();
        });

        await InScope(host, async services =>
        {
            var stored = await services.GetRequiredService<IAgentRunRepository>().GetByIdAsync(run.Id);
            Assert.Equal("AwaitingApproval", stored.Status.Value);
            Assert.Equal(new[] { 1, 2, 3 }, stored.Plan.Calls.Select(c => c.Index));
            // started, three calls, completed
            Assert.Equal(5, stored.Version);
        });
    }

    [Fact]
    public async Task SecondChangeFromAnOlderCopyOfARunFails()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var run = NewRun();
        run.ProposeCall(new ProposedCall("BobsPhone_Ping", "{}"));
        run.CompletePlan();
        await InScope(host, async services => await services.GetRequiredService<IAgentRunRepository>().AddAsync(run));

        await using var first = host.OrchestrationRoot.CreateAsyncScope();
        await using var second = host.OrchestrationRoot.CreateAsyncScope();
        var firstCopy = await first.ServiceProvider.GetRequiredService<IAgentRunRepository>().GetByIdAsync(run.Id);
        var secondCopy = await second.ServiceProvider.GetRequiredService<IAgentRunRepository>().GetByIdAsync(run.Id);

        firstCopy.ApprovePlan(run.AuthorId);
        await first.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();

        secondCopy.RejectPlan(run.AuthorId);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            second.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync());

        await InScope(host, async services =>
            Assert.Equal("Approved", (await services.GetRequiredService<IAgentRunRepository>().GetByIdAsync(run.Id)).Status.Value));
    }

    private static AgentRun NewRun()
    {
        var phone = new ParticipantSnapshot(Guid.NewGuid(), "Bob's phone",
        [
            new FunctionSnapshot(Guid.NewGuid(), Guid.NewGuid(), ReadFile),
            new FunctionSnapshot(Guid.NewGuid(), Guid.NewGuid(), SaveFile),
            new FunctionSnapshot(Guid.NewGuid(), Guid.NewGuid(), Ping)
        ]);
        var group = Group.CreateDefault(new GroupId(Guid.NewGuid()), new ParticipantId(phone.ParticipantId));

        return group.StartAgentRun([phone], new ParticipantId(phone.ParticipantId), Guid.NewGuid(), "copy my notes");
    }

    private static async Task InScope(ModulesHost host, Func<IServiceProvider, Task> work)
    {
        await using var scope = host.OrchestrationRoot.CreateAsyncScope();
        await work(scope.ServiceProvider);
        await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();
    }

    private static string Json<T>(T value) => JsonSerializer.Serialize(value);
}
