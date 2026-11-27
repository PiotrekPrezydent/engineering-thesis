using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantContext;

public class UpdateParticipantContextCommandHandler : ICommandHandler<UpdateParticipantContextCommand>
{
    private readonly IParticipantRepository _repository;

    public UpdateParticipantContextCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateParticipantContextCommand command)
    {
        var participant = await _repository.GetByIdAsync(new(command.ParticipantId));
        participant.ChangeContext(command.Context);
    }
}
