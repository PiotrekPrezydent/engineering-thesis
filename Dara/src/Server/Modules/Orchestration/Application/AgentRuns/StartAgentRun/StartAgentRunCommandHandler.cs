using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

// only saves the run, the model is called later from the outbox
public class StartAgentRunCommandHandler : ICommandHandler<StartAgentRunCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IAgentRunRepository _agentRunRepository;

    public StartAgentRunCommandHandler(IGroupRepository groupRepository, IParticipantRepository participantRepository, IAgentRunRepository agentRunRepository)
    {
        _groupRepository = groupRepository;
        _participantRepository = participantRepository;
        _agentRunRepository = agentRunRepository;
    }

    public async Task HandleAsync(StartAgentRunCommand command)
    {
        var group = await _groupRepository.GetByIdAsync(new GroupId(command.GroupId));

        // an ordinary message of the group
        var prompt = group.FindPrompt(command.Message);
        if (prompt is null)
            return;

        var participants = await _participantRepository.GetByIdsAsync(group.Participants);

        var agentRun = group.StartAgentRun(
            participants.Select(p => p.ToSnapshot()).ToList(),
            new ParticipantId(command.AuthorId),
            command.MessageId,
            prompt);

        await _agentRunRepository.AddAsync(agentRun);
    }
}
