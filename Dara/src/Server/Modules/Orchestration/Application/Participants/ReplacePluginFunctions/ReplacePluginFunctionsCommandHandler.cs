using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Application.Participants.ReplacePluginFunctions;

public class ReplacePluginFunctionsCommandHandler : ICommandHandler<ReplacePluginFunctionsCommand>
{
    private readonly IParticipantRepository _repository;

    public ReplacePluginFunctionsCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ReplacePluginFunctionsCommand command)
    {
        var participant = await GetOrCreateParticipantAsync(new ParticipantId(command.ParticipantId));

        var functions = command.Functions
            .Select(f => new AvailableFunction(new FunctionId(f.FunctionId), FunctionDescriptionMapper.ToDomain(f.Description)))
            .ToList();

        participant.ReplacePluginFunctions(new PluginId(command.PluginId), command.Version, functions);
    }

    // a client can register plugins before the new user event reaches this module
    private async Task<Participant> GetOrCreateParticipantAsync(ParticipantId participantId)
    {
        var participant = await _repository.FindByIdAsync(participantId);
        if (participant is not null)
            return participant;

        participant = Participant.CreateDefault(participantId);
        await _repository.AddAsync(participant);

        return participant;
    }
}
