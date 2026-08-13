using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantName;

public class UpdateParticipantNameCommandHandler : ICommandHandler<UpdateParticipantNameCommand>
{
    private readonly IParticipantRepository _repository;

    public UpdateParticipantNameCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateParticipantNameCommand command)
    {
        var participant = await _repository.GetByIdAsync(new(command.ParticipantId));
        participant.UpdateName(command.NewName);
    }
}