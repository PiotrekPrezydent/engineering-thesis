using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Participants.CreateParticipant;

public class CreateParticipantCommandHandler : ICommandHandler<CreateParticipantCommand>
{
    private readonly IParticipantRepository _repository;

    public CreateParticipantCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateParticipantCommand command)
    {
        // the participant exists already when plugin functions came before this event, it gets the data of the client
        var existing = await _repository.FindByIdAsync(new(command.ParticipantId));
        if (existing is not null)
        {
            existing.UpdateName(command.Name);
            return;
        }

        var participant = Participant.Create(new(command.ParticipantId), command.Name);
        await _repository.AddAsync(participant);
    }
}
