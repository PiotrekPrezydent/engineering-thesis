using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

// one prompt to a plan the author approves or rejects as a whole, on snapshots of what the model saw
public class AgentRun : Entity, IAggregateRoot
{
    public AgentRunId Id { get; private set; }
    
    public GroupId GroupId { get; private set; }
    public ParticipantId AuthorId { get; private set; }
    public Guid MessageId { get; private set; }
    public string Prompt { get; private set; }

    public AgentRunStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    
    public DateTime StartedAt { get; private set; }
    public DateTime? PlanCreatedAt { get; private set; }
    public DateTime? DecidedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }

    public GroupSnapshot GroupSnapshot { get; private set; }
    public IReadOnlyList<ParticipantSnapshot> ParticipantsSnapshots { get; private set; }
    public IReadOnlyList<ToolAlias> ToolAliases { get; private set; }
    
    public ExecutionPlan Plan { get; private set; }

    // concurrency token, every change of the run raises it, so two commands cannot both change one run
    public int Version { get; private set; }

    private AgentRun()
    {
    }

    private AgentRun(GroupSnapshot group, IReadOnlyList<ParticipantSnapshot> participants, ParticipantId authorId, Guid messageId, string prompt)
    {
        Id = new AgentRunId(Guid.NewGuid());
        GroupId = new GroupId(group.GroupId);
        AuthorId = authorId;
        MessageId = messageId;
        Prompt = prompt;

        Status = AgentRunStatus.PlanGenerating;
        StartedAt = DateTime.UtcNow;

        // only members of the group can get calls of the plan
        GroupSnapshot = group;
        ParticipantsSnapshots = participants.Where(p => group.ParticipantsIds.Contains(p.ParticipantId)).ToList();
        ToolAliases = ToolAliasBuilder.Build(ParticipantsSnapshots);

        Plan = ExecutionPlan.CreateEmpty(Id);
        Version = 1;

        AddDomainEvent(new AgentRunStartedDomainEvent(Id, GroupId, AuthorId, MessageId));
        AddDomainEvent(new PlanGenerationRequestedDomainEvent(Id));
    }
    
    internal static AgentRun Start(GroupSnapshot group, IReadOnlyList<ParticipantSnapshot> participants, ParticipantId authorId, Guid messageId, string prompt)
    {
        return new AgentRun(group, participants, authorId, messageId, prompt);
    }

    // a run that left plan generation was handled by an earlier notification
    public bool IsGeneratingPlan()
    {
        return Status == AgentRunStatus.PlanGenerating;
    }

    // the function behind a tool as the model saw it when the run started
    public FunctionSnapshot? FindFunction(ToolAlias tool)
    {
        return ParticipantsSnapshots
            .FirstOrDefault(p => p.ParticipantId == tool.ParticipantId.Value)?
            .Functions.FirstOrDefault(f => f.FunctionId == tool.FunctionId.Value);
    }

    // returns the index the model uses as {{result:N}}, a broken rule tells the model what to fix
    public int ProposeCall(ProposedCall call)
    {
        CheckRule(new AgentRunMustBeInStatusRule(Status, AgentRunStatus.PlanGenerating, "add a call to the plan"));
        CheckRule(new ProposedCallMustUseKnownToolRule(ToolAliases, call.Alias));
        CheckRule(new ProposedArgumentsMustBeJsonObjectRule(call.ArgumentsJson));

        var tool = ToolAliases.First(t => string.Equals(t.Alias, call.Alias, StringComparison.OrdinalIgnoreCase));
        var function = FindFunction(tool)!.Description;
        var arguments = CallArgumentParser.Parse(call.ArgumentsJson);

        CheckRule(new CallArgumentsMustMatchFunctionRule(function, arguments));
        CheckRule(new ResultReferenceMustPointToEarlierCallRule(Plan.NextIndex, arguments));
        CheckRule(new ResultTypeMustMatchParameterRule(function, arguments, EarlierResults()));

        var planned = Plan.AddCall(tool, arguments);
        Version++;

        return planned.Index;
    }

    public void CompletePlan()
    {
        CheckRule(new AgentRunMustBeInStatusRule(Status, AgentRunStatus.PlanGenerating, "complete the plan"));
        CheckRule(new ExecutionPlanMustHaveCallsRule(Plan));

        Status = AgentRunStatus.AwaitingApproval;
        PlanCreatedAt = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new ExecutionPlanCreatedDomainEvent(Id, GroupId, AuthorId));
    }

    // a failure is a state, not an exception, so the outbox loop does not call the model again
    public void FailPlanGeneration(string reason)
    {
        CheckRule(new AgentRunMustBeInStatusRule(Status, AgentRunStatus.PlanGenerating, "fail the plan generation"));

        Status = AgentRunStatus.PlanGenerationFailed;
        FailureReason = reason;
        FailedAt = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new PlanGenerationFailedDomainEvent(Id, GroupId, AuthorId, reason));
    }

    public void ApprovePlan(ParticipantId deciderId)
    {
        CheckRule(new OnlyAuthorCanDecideAboutPlanRule(AuthorId, deciderId));
        CheckRule(new AgentRunMustBeInStatusRule(Status, AgentRunStatus.AwaitingApproval, "approve the plan"));

        Status = AgentRunStatus.Approved;
        DecidedAt = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new ExecutionPlanApprovedDomainEvent(Id, GroupId, AuthorId));
    }

    public void RejectPlan(ParticipantId deciderId)
    {
        CheckRule(new OnlyAuthorCanDecideAboutPlanRule(AuthorId, deciderId));
        CheckRule(new AgentRunMustBeInStatusRule(Status, AgentRunStatus.AwaitingApproval, "reject the plan"));

        Status = AgentRunStatus.Rejected;
        DecidedAt = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new AgentRunRejectedDomainEvent(Id, GroupId, AuthorId));
    }

    private IReadOnlyDictionary<int, TypeDescription?> EarlierResults()
    {
        return Plan.Calls.ToDictionary(
            c => c.Index,
            c => FindFunction(new ToolAlias(c.Alias, c.ParticipantId, c.FunctionId))?.Description.Returns);
    }
}