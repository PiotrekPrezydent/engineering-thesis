using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Participants.CreateParticipant;

public class CreateDefaultParticipantCommandHandler : ICommandHandler<CreateDefaultParticipantCommand>
{
    private readonly IParticipantRepository _repository;

    public CreateDefaultParticipantCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateDefaultParticipantCommand command)
    {
        // the participant exists already when plugin functions came before this event
        if (await _repository.FindByIdAsync(new(command.ParticipantId)) is not null)
            return;

        var participant = Participant.CreateDefault(new(command.ParticipantId));
        await _repository.AddAsync(participant);
    }
}