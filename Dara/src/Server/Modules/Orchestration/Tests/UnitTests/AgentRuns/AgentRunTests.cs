using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;
using Dara.Server.Modules.Orchestration.Domain.Groups.Rules;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using static Dara.Server.Modules.Orchestration.UnitTests.OrchestrationSamples;

namespace Dara.Server.Modules.Orchestration.UnitTests.AgentRuns;

// PlanGenerating -> AwaitingApproval -> Approved | Rejected, or PlanGenerating -> PlanGenerationFailed
public class AgentRunTests
{
    [Fact]
    public void StartedRunGeneratesAPlanFromSnapshotsOfGroupMembers()
    {
        var phone = Phone;
        var laptop = Laptop;
        var stranger = Client("Stranger", Ping);
        var group = GroupOf(phone, laptop);
        var messageId = Guid.NewGuid();

        var run = group.StartAgentRun([phone, laptop, stranger], AuthorOf(phone), messageId, "save my battery level");

        Assert.Equal("PlanGenerating", run.Status.Value);
        Assert.Equal(group.Id, run.GroupId);
        Assert.Equal("save my battery level", run.Prompt);
        Assert.Equal(new[] { phone.ParticipantId, laptop.ParticipantId }, run.ParticipantsSnapshots.Select(p => p.ParticipantId));
        Assert.Equal(8, run.ToolAliases.Count);
        Assert.Empty(run.Plan.Calls);
        Assert.Equal(1, run.Version);

        var started = Assert.IsType<AgentRunStartedDomainEvent>(run.DomainEvents.First());
        Assert.Equal(run.Id, started.AgentRunId);
        Assert.Equal(messageId, started.MessageId);
        Assert.Equal(AuthorOf(phone), started.AuthorId);
        Assert.IsType<PlanGenerationRequestedDomainEvent>(run.DomainEvents.Last());
    }

    [Fact]
    public void OnlyAGroupParticipantStartsARunWithAPrompt()
    {
        var phone = Phone;
        var group = GroupOf(phone);

        AssertBroken<AuthorMustBeGroupParticipantRule>(() =>
            group.StartAgentRun([phone], new ParticipantId(Guid.NewGuid()), Guid.NewGuid(), "hello"));
        AssertBroken<PromptMustBeProvidedRule>(() =>
            group.StartAgentRun([phone], AuthorOf(phone), Guid.NewGuid(), " "));
    }

    [Fact]
    public void FunctionOfAToolIsTheOneFromTheSnapshot()
    {
        var run = StartRun(out var phone, out _);

        var function = run.FindFunction(run.ToolAliases.Single(a => a.Alias == "BobsPhone_SaveFile"));

        Assert.NotNull(function);
        Assert.Equal(phone.Functions.Single(f => f.Description.Name == "SaveFile"), function);
    }

    [Fact]
    public void CompletedPlanWaitsForTheAuthor()
    {
        var run = StartRun();
        run.Propose("BobsPhone_GetBatteryLevel");
        run.ClearDomainEvents();

        run.CompletePlan();

        Assert.Equal("AwaitingApproval", run.Status.Value);
        Assert.NotNull(run.PlanCreatedAt);
        var created = Assert.IsType<ExecutionPlanCreatedDomainEvent>(Assert.Single(run.DomainEvents));
        Assert.Equal(run.AuthorId, created.AuthorId);
    }

    [Fact]
    public void PlanWithoutCallsCannotBeCompleted()
    {
        AssertBroken<ExecutionPlanMustHaveCallsRule>(() => StartRun().CompletePlan());
    }

    [Fact]
    public void FailedGenerationKeepsTheReasonAndTakesNoMoreCalls()
    {
        var run = StartRun();
        run.ClearDomainEvents();

        run.FailPlanGeneration("the model answered with text only");

        Assert.Equal("PlanGenerationFailed", run.Status.Value);
        Assert.Equal("the model answered with text only", run.FailureReason);
        Assert.NotNull(run.FailedAt);
        var failed = Assert.IsType<PlanGenerationFailedDomainEvent>(Assert.Single(run.DomainEvents));
        Assert.Equal("the model answered with text only", failed.Reason);

        AssertBroken<AgentRunMustBeInStatusRule>(() => run.Propose("BobsPhone_Ping"));
        AssertBroken<AgentRunMustBeInStatusRule>(() => run.CompletePlan());
        AssertBroken<AgentRunMustBeInStatusRule>(() => run.FailPlanGeneration("again"));
    }

    [Fact]
    public void AuthorApprovesTheWholePlan()
    {
        var run = AwaitingApproval();

        run.ApprovePlan(run.AuthorId);

        Assert.Equal("Approved", run.Status.Value);
        Assert.NotNull(run.DecidedAt);
        Assert.IsType<ExecutionPlanApprovedDomainEvent>(Assert.Single(run.DomainEvents));
    }

    [Fact]
    public void AuthorRejectsTheWholePlan()
    {
        var run = AwaitingApproval();

        run.RejectPlan(run.AuthorId);

        Assert.Equal("Rejected", run.Status.Value);
        Assert.NotNull(run.DecidedAt);
        Assert.IsType<AgentRunRejectedDomainEvent>(Assert.Single(run.DomainEvents));
    }

    [Fact]
    public void OnlyTheAuthorDecides()
    {
        var run = StartRun(out _, out var laptop);
        run.Propose("BobsPhone_Ping");
        run.CompletePlan();

        AssertBroken<OnlyAuthorCanDecideAboutPlanRule>(() => run.ApprovePlan(AuthorOf(laptop)));
        AssertBroken<OnlyAuthorCanDecideAboutPlanRule>(() => run.RejectPlan(AuthorOf(laptop)));
        Assert.Equal("AwaitingApproval", run.Status.Value);
    }

    [Fact]
    public void DecisionIsTakenOnceAndOnlyAboutAReadyPlan()
    {
        var generating = StartRun();
        AssertBroken<AgentRunMustBeInStatusRule>(() => generating.ApprovePlan(generating.AuthorId));
        AssertBroken<AgentRunMustBeInStatusRule>(() => generating.RejectPlan(generating.AuthorId));

        var approved = AwaitingApproval();
        approved.ApprovePlan(approved.AuthorId);
        AssertBroken<AgentRunMustBeInStatusRule>(() => approved.ApprovePlan(approved.AuthorId));
        AssertBroken<AgentRunMustBeInStatusRule>(() => approved.RejectPlan(approved.AuthorId));
        AssertBroken<AgentRunMustBeInStatusRule>(() => approved.Propose("BobsPhone_Ping"));

        var rejected = AwaitingApproval();
        rejected.RejectPlan(rejected.AuthorId);
        AssertBroken<AgentRunMustBeInStatusRule>(() => rejected.ApprovePlan(rejected.AuthorId));
    }

    [Fact]
    public void EveryChangeRaisesTheVersion()
    {
        var run = StartRun();

        run.Propose("BobsPhone_Ping");
        Assert.Equal(2, run.Version);

        run.CompletePlan();
        Assert.Equal(3, run.Version);

        run.ApprovePlan(run.AuthorId);
        Assert.Equal(4, run.Version);
    }

    [Fact]
    public void BrokenRuleChangesNothing()
    {
        var run = StartRun();

        AssertBroken<ProposedCallMustUseKnownToolRule>(() => run.Propose("Nobody_Ping"));

        Assert.Empty(run.Plan.Calls);
        Assert.Equal(1, run.Version);
    }

    private static AgentRun AwaitingApproval()
    {
        var run = StartRun();
        run.Propose("BobsPhone_Ping");
        run.CompletePlan();
        run.ClearDomainEvents();
        return run;
    }
}
