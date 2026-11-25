using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Application.Participants;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Groups;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// no transaction is open while the model answers, the run is saved once after the loop
public class GenerateExecutionPlanCommandHandler : ICommandHandler<GenerateExecutionPlanCommand>
{
    private readonly IAgentRunRepository _agentRunRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IExecutionPlanGenerator _generator;

    public GenerateExecutionPlanCommandHandler(IAgentRunRepository agentRunRepository, IGroupRepository groupRepository, IExecutionPlanGenerator generator)
    {
        _agentRunRepository = agentRunRepository;
        _groupRepository = groupRepository;
        _generator = generator;
    }

    public async Task HandleAsync(GenerateExecutionPlanCommand command)
    {
        var agentRun = await _agentRunRepository.GetByIdAsync(new AgentRunId(command.AgentRunId));

        // a repeated notification does not call the model again
        if (!agentRun.IsGeneratingPlan())
            return;

        // the run never keeps the api key, it is read from the group when the model is called
        var group = await _groupRepository.GetByIdAsync(agentRun.GroupId);

        var result = await _generator.GenerateAsync(CreateRequest(agentRun, group.ApiKey), new AgentRunCallRecorder(agentRun));

        if (result.Succeeded)
            agentRun.CompletePlan();
        else
            agentRun.FailPlanGeneration(result.FailureReason ?? "the plan could not be generated");
    }

    private static ExecutionPlanGenerationRequest CreateRequest(AgentRun agentRun, string apiKey)
    {
        var names = agentRun.ParticipantsSnapshots.ToDictionary(p => p.ParticipantId, p => p.Name);

        var tools = agentRun.ToolAliases
            .Select(tool => new PlanningTool(
                tool.Alias,
                names[tool.ParticipantId.Value],
                FunctionDescriptionMapper.ToContract(agentRun.FindFunction(tool)!.Description)))
            .ToList();

        return new ExecutionPlanGenerationRequest(
            agentRun.GroupSnapshot.ChatClientProviderName,
            agentRun.GroupSnapshot.ClientModelName,
            apiKey,
            names.GetValueOrDefault(agentRun.AuthorId.Value, "unknown client"),
            agentRun.Prompt,
            names.Values.ToList(),
            tools);
    }
}
